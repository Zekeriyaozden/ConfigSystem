using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Config.Data.Ef;

[Index("ApplicationName", "Name", Name = "IX_Config_AppName_Name", IsUnique = true)]
public partial class Configuration
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string Type { get; set; } = null!;

    [StringLength(500)]
    public string Value { get; set; } = null!;

    public bool IsActive { get; set; }

    [StringLength(100)]
    public string ApplicationName { get; set; } = null!;

    public DateTime LastUpdated { get; set; }
}
