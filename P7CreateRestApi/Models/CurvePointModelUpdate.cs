using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class CurvePointModelUpdate
    {
        [Required(ErrorMessage = "CurveId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CurveId must be a positive integer.")]
        public byte? CurveId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? AsOfDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Term must be a positive number.")]
        public double? Term { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "CurvePointValue must be a positive number.")]
        public double? CurvePointValue { get; set; }
    }
}
