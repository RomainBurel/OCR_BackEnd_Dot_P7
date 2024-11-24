using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class BidListModelUpdate
    {
        [Required(ErrorMessage = "Account is required.")]
        [StringLength(255)]
        public string Account { get; set; }

        [Required(ErrorMessage = "BidType is required.")]
        [StringLength(255)]
        public string BidType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BidQuantity must be a positive number.")]
        public double? BidQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "AskQuantity must be a positive number.")]
        public double? AskQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bid must be a positive number.")]
        public double? Bid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ask must be a positive number.")]
        public double? Ask { get; set; }

        [Required(ErrorMessage = "Benchmark is required.")]
        [StringLength(255)]
        public string Benchmark { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? BidListDate { get; set; }

        [StringLength(500)]
        public string Commentary { get; set; }

        [Required(ErrorMessage = "BidSecurity is required.")]
        [StringLength(255)]
        public string BidSecurity { get; set; }

        [Required(ErrorMessage = "BidStatus is required.")]
        [StringLength(255)]
        public string BidStatus { get; set; }

        [Required(ErrorMessage = "Trader is required.")]
        [StringLength(255)]
        public string Trader { get; set; }

        [Required(ErrorMessage = "Book is required.")]
        [StringLength(255)]
        public string Book { get; set; }

        [Required(ErrorMessage = "RevisionName is required.")]
        [StringLength(255)]
        public string RevisionName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? RevisionDate { get; set; }

        [Required(ErrorMessage = "DealName is required.")]
        [StringLength(255)]
        public string DealName { get; set; }

        [Required(ErrorMessage = "DealType is required.")]
        [StringLength(255)]
        public string DealType { get; set; }

        [Required(ErrorMessage = "SourceListId is required.")]
        [StringLength(255)]
        public string SourceListId { get; set; }

        [Required(ErrorMessage = "Side is required.")]
        [StringLength(255)]
        public string Side { get; set; }
    }
}
