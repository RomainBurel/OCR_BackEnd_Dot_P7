﻿namespace P7CreateRestApi.Models
{
    public class RatingModel
    {
        public int Id { get; set; }
        public string MoodysRating { get; set; } = string.Empty;
        public string SandPRating { get; set; } = string.Empty;
        public string FitchRating { get; set; } = string.Empty;
        public byte? OrderNumber { get; set; }
    }
}
