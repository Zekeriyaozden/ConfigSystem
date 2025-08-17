using Config.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Config.Data.Ef;

public sealed class EfConfigStore : IConfigStore
{
    private readonly DbContextOptions<ConfigDbContext> _options;

    public EfConfigStore(string connectionString)
    {
        var b = new DbContextOptionsBuilder<ConfigDbContext>();
        b.UseSqlServer(connectionString);
        _options = b.Options;
    }

    public async Task<IReadOnlyList<ConfigItem>> GetActiveAsync(string applicationName, CancellationToken ct)
    {
        await using var db = new ConfigDbContext(_options);

        var rows = await db.Configuration
            .AsNoTracking()
            .Where(c => c.IsActive && c.ApplicationName == applicationName)
            .Select(c => new ConfigItem(
                c.Id,c.Name, c.Type, c.Value, c.IsActive, c.ApplicationName, c.LastUpdated
            ))
            .ToListAsync(ct);

        return rows.AsReadOnly();
    }
}
