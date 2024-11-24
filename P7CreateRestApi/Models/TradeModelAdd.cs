using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class TradeModelAdd
    {
        [Required(ErrorMessage = "Account is required.")]
        [StringLength(255)]
        public string Account { get; set; }

        [Required(ErrorMessage = "AccountType is required.")]
        [StringLength(255)]
        public string AccountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BuyQuantity must be a positive number.")]
        public double? BuyQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SellQuantity must be a positive number.")]
        public double? SellQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "BuyPrice must be a positive number.")]
        public double? BuyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SellPrice must be a positive number.")]
        public double? SellPrice { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? TradeDate { get; set; }

        [Required(ErrorMessage = "TradeSecurity is required.")]
        [StringLength(255)]
        public string TradeSecurity { get; set; }

        [Required(ErrorMessage = "TradeStatus is required.")]
        [StringLength(255)]
        public string TradeStatus { get; set; }

        [Required(ErrorMessage = "Trader is required.")]
        [StringLength(255)]
        public string Trader { get; set; }

        [Required(ErrorMessage = "Benchmark is required.")]
        [StringLength(255)]
        public string Benchmark { get; set; }

        [Required(ErrorMessage = "Book is required.")]
        [StringLength(255)]
        public string Book { get; set; }

        [Required(ErrorMessage = "CreationName is required.")]
        [StringLength(255)]
        public string CreationName { get; set; }
        
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? CreationDate { get; set; }

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
