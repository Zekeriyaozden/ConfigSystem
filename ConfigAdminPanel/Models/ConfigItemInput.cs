using System.ComponentModel.DataAnnotations;

namespace ConfigAdminPanel.Models
{
    public sealed class ConfigItemInput
    {
        public int Id { get; set; }
        [Required] public string ApplicationName { get; set; } = "";
        [Required] public string Name { get; set; } = "";
        [Required] public string Type { get; set; } = "string";
        [Required] public string Value { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
