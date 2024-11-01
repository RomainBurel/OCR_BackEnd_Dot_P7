using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RuleNameModelAdd
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Json { get; set; } = string.Empty;
        [Required]
        public string Template { get; set; } = string.Empty;
        [Required]
        public string SqlStr { get; set; } = string.Empty;
        [Required]
        public string SqlPart { get; set; } = string.Empty;
    }
}
