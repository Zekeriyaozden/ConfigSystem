using System.Threading;

namespace Config.Abstractions;

public interface IConfigRepository
{
    Task<IReadOnlyList<ConfigItem>> ListAsync(
        string? applicationName = null,
        bool? onlyActive = null,
        CancellationToken ct = default);

    Task<ConfigItem?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(ConfigItem item, CancellationToken ct = default);

    Task<bool> UpdateAsync(ConfigItem item, CancellationToken ct = default);

    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
