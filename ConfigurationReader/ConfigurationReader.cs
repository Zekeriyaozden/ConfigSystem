using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Config.Abstractions;

namespace ConfigurationReader;

public sealed class ConfigurationReader : IDisposable
{
    private readonly string _applicationName;
    private readonly TimeSpan _refreshInterval;
    private readonly IConfigStore _store;

    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _refreshGate = new(1, 1);
    private Task? _loop;

    private readonly struct ConfigEntry
    {
        public ConfigEntry(string type, string value)
        {
            Type = type ?? "string";
            Value = value ?? "";
        }
        public string Type { get; }
        public string Value { get; }
    }

    private ImmutableDictionary<string, ConfigEntry> _snapshot =
        ImmutableDictionary<string, ConfigEntry>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

    public ConfigurationReader(string applicationName, int refreshTimerIntervalInMs, IConfigStore store)
    {
        _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        if (refreshTimerIntervalInMs <= 0) throw new ArgumentOutOfRangeException(nameof(refreshTimerIntervalInMs));
        _refreshInterval = TimeSpan.FromMilliseconds(refreshTimerIntervalInMs);
        _store = store ?? throw new ArgumentNullException(nameof(store));

        _loop = Task.Run(RefreshLoopAsync);
    }

    public T GetValue<T>(string key)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));

        if (_snapshot.TryGetValue(key, out var entry))
            return ConvertStringTo<T>(entry.Value);

        throw new KeyNotFoundException($"Config key not found: '{key}' for '{_applicationName}'.");
    }

    public object GetValue(string key)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));

        if (_snapshot.TryGetValue(key, out var entry))
            return ConvertByTypeName(entry.Type, entry.Value);

        throw new KeyNotFoundException($"Config key not found: '{key}' for '{_applicationName}'.");
    }


    private async Task RefreshLoopAsync()
    {
        await SafeRefreshOnceAsync();
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_refreshInterval, _cts.Token);
                await SafeRefreshOnceAsync();
            }
            catch (OperationCanceledException) { }
        }
    }

    private async Task SafeRefreshOnceAsync()
    {
        if (!await _refreshGate.WaitAsync(0)) return;

        try
        {
            var rows = await _store.GetActiveAsync(_applicationName, _cts.Token);

            var builder = ImmutableDictionary.CreateBuilder<string, ConfigEntry>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in rows)
            {
                builder[r.Name] = new ConfigEntry(r.Type, r.Value);
            }

            Interlocked.Exchange(ref _snapshot, builder.ToImmutable());
        }
        catch
        {
            // fallback: eski snapshot ile devam
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    private static T ConvertStringTo<T>(string raw)
    {
        var t = typeof(T);
        if (t == typeof(string)) return (T)(object)raw;

        if (t == typeof(int))
        {
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                return (T)(object)i;
            throw new InvalidCastException($"'{raw}'");
        }

        if (t == typeof(bool))
        {
            if (raw is "1") return (T)(object)true;
            if (raw is "0") return (T)(object)false;
            if (bool.TryParse(raw, out var b)) return (T)(object)b;
            throw new InvalidCastException($"'{raw}' ");
        }

        if (t == typeof(double))
        {
            if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var d))
                return (T)(object)d;
            throw new InvalidCastException($"'{raw}'");
        }

        return (T)Convert.ChangeType(raw, t, CultureInfo.InvariantCulture);
    }

    private static object ConvertByTypeName(string typeName, string raw)
    {
        switch (typeName.Trim().ToLowerInvariant())
        {
            case "bool":
            case "boolean":
                if (raw is "1") return true;
                if (raw is "0") return false;
                if (bool.TryParse(raw, out var b)) return b;
                throw new InvalidCastException($"'{raw}'");

            case "int":
            case "int32":
                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                    return i;
                throw new InvalidCastException($"'{raw}'");

            case "double":
            case "float64":
                if (double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var d))
                    return d;
                throw new InvalidCastException($"'{raw}' ");

            case "string":
            default:
                return raw;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        try { _loop?.Wait(1000); } catch { }
        _cts.Dispose();
        _refreshGate.Dispose();
    }
}
