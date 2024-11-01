using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class UserModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
