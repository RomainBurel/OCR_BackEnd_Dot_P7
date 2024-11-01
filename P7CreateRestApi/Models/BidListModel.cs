using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class BidListModel
    {
        [Required]
        public int BidListId { get; set; }
        [Required]
        public string Account { get; set; }
        [Required]
        public string BidType { get; set; }
        public double? BidQuantity { get; set; }
        public double? AskQuantity { get; set; }
        public double? Bid { get; set; }
        public double? Ask { get; set; }
        [Required]
        public string Benchmark { get; set; }
        public DateTime? BidListDate { get; set; }
        [Required]
        public string Commentary { get; set; }
        [Required]
        public string BidSecurity { get; set; }
        [Required]
        public string BidStatus { get; set; }
        [Required]
        public string Trader { get; set; }
        [Required]
        public string Book { get; set; }
        [Required]
        public string CreationName { get; set; }
        public DateTime? CreationDate { get; set; }
        [Required]
        public string RevisionName { get; set; }
        public DateTime? RevisionDate { get; set; }
        [Required]
        public string DealName { get; set; }
        [Required]
        public string DealType { get; set; }
        [Required]
        public string SourceListId { get; set; }
        [Required]
        public string Side { get; set; }
    }
}
