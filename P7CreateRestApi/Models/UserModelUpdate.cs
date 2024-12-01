using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class UserModelUpdate
    {
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(255)]
        public string FullName { get; set; }
    }
}
