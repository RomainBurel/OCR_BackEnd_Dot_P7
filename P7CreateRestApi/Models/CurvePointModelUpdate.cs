namespace P7CreateRestApi.Models
{
    public class CurvePointModelUpdate
    {
        public byte? CurveId { get; set; }
        public DateTime? AsOfDate { get; set; }
        public double? Term { get; set; }
        public double? CurvePointValue { get; set; }
    }
}
