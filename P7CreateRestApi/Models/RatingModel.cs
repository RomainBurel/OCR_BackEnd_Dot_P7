using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RatingModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string MoodysRating { get; set; }
        [Required]
        public string SandPRating { get; set; }
        [Required]
        public string FitchRating { get; set; }
        public byte? OrderNumber { get; set; }
    }
}
