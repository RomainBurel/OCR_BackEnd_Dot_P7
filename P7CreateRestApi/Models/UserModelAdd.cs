using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class UserModelAdd
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
