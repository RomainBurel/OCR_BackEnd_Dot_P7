using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Models
{
    public class RuleNameModelUpdate
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Json is required.")]
        public string Json { get; set; }

        [Required(ErrorMessage = "Template is required.")]
        public string Template { get; set; }

        [Required(ErrorMessage = "SqlStr is required.")]
        public string SqlStr { get; set; }

        [Required(ErrorMessage = "SqlPart is required.")]
        public string SqlPart { get; set; }
    }
}
