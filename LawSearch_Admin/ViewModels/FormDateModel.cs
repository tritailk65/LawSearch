using System.ComponentModel.DataAnnotations;

namespace LawSearch_Admin.ViewModels
{
    public class FormDateModel
    {
        [Required]
        [DateRange]
        public DateTime fromDate { get; set; }
        [Required]
        public DateTime toDate { get; set; }
    }
}
