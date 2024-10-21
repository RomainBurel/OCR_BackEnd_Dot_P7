namespace P7CreateRestApi.Models
{
    public class RatingModelAdd
    {
        public string MoodysRating { get; set; } = string.Empty;
        public string SandPRating { get; set; } = string.Empty;
        public string FitchRating { get; set; } = string.Empty;
        public byte? OrderNumber { get; set; }
    }
}
