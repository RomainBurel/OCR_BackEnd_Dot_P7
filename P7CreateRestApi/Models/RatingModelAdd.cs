using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RatingModelAdd
    {
        [Required(ErrorMessage = "MoodysRating is required.")]
        [StringLength(255)]
        public string MoodysRating { get; set; }
        
        [Required(ErrorMessage = "SandPRating is required.")]
        [StringLength(255)]
        public string SandPRating { get; set; }
        
        [Required(ErrorMessage = "FitchRating is required.")]
        [StringLength(255)]
        public string FitchRating { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "OrderNumber must be a positive integer.")]
        public byte? OrderNumber { get; set; }
    }
}
