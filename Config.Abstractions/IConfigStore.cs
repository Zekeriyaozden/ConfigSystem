using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Config.Abstractions
{
    public interface IConfigStore
    {
        Task<IReadOnlyList<ConfigItem>> GetActiveAsync(string applicationName, CancellationToken ct);
    }
}
