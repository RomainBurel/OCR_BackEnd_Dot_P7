using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class UserModelAdd
    {
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,50}$", ErrorMessage = "Password must contain between 8 and 50 characters with at least one uppercase, one lowercase character, one number and one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(255)]
        public string FullName { get; set; }
    }
}
