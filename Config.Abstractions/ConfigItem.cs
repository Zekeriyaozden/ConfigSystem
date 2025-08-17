using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.Abstractions;

public sealed record ConfigItem(
    int Id,
    string Name,
    string Type,
    string Value,
    bool IsActive,
    string ApplicationName,
    DateTime LastUpdatedUtc
);
