using Config.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Config.Data.Ef;

public sealed class ConfigRepository : IConfigRepository
{
    private readonly ConfigDbContext _db;
    public ConfigRepository(ConfigDbContext db) => _db = db;

    public async Task<ConfigItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await _db.Configuration.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return e is null ? null : new ConfigItem(
            e.Id, e.Name, e.Type, e.Value, e.IsActive, e.ApplicationName, e.LastUpdated);
    }

    public async Task<int> CreateAsync(ConfigItem item, CancellationToken ct = default)
    {
        ValidateTypeValue(item.Type, item.Value);

        var e = new Configuration
        {
            ApplicationName = item.ApplicationName,
            Name = item.Name,
            Type = item.Type,
            Value = item.Value,
            IsActive = item.IsActive,
            LastUpdated = DateTime.UtcNow
        };

        _db.Configuration.Add(e);
        await _db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task<bool> UpdateAsync(ConfigItem item, CancellationToken ct = default)
    {
        var e = await _db.Configuration.FirstOrDefaultAsync(x => x.Id == item.Id, ct);
        if (e is null) return false;

        ValidateTypeValue(item.Type, item.Value);

        e.ApplicationName = item.ApplicationName;
        e.Name = item.Name;
        e.Type = item.Type;
        e.Value = item.Value;
        e.IsActive = item.IsActive;
        e.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    // Soft delete = IsActive=false
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await _db.Configuration.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;

        e.IsActive = false;
        e.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static void ValidateTypeValue(string type, string value)
    {
        if (type.Equals("bool", StringComparison.OrdinalIgnoreCase))
        {
            if (!(value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value.Equals("false", StringComparison.OrdinalIgnoreCase)
               || value == "1" || value == "0"))
                throw new InvalidOperationException("Bool value must be true/false or 1/0");
        }
        else if (type.Equals("int", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(value, out _))
                throw new InvalidOperationException("Value must be an integer");
        }
        else if (type.Equals("double", StringComparison.OrdinalIgnoreCase))
        {
            if (!double.TryParse(value,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _))
                throw new InvalidOperationException("Value must be a double (use dot as decimal separator)");
        }
    }

    public async Task<IReadOnlyList<ConfigItem>> ListAsync(string? applicationName = null, bool? onlyActive = null, CancellationToken ct = default)
    {
        var q = _db.Configuration.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(applicationName))
            q = q.Where(x => x.ApplicationName == applicationName);


        var rows = await q
            .OrderBy(x => x.ApplicationName).ThenBy(x => x.Name)
            .Select(x => new ConfigItem(
                x.Id,
                x.Name,
                x.Type,
                x.Value,
                x.IsActive,
                x.ApplicationName,
                x.LastUpdated
            ))
            .ToListAsync(ct);

        return rows.AsReadOnly();
    }
}
