using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Models.Periods
{
    public class PeriodVM 
    {
        public int Id { get; set; }
        [Display(Name = "Naziv perioda")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Početak perioda")]
        public DateOnly StartDate { get; set; }

        [Required]
        [Display(Name = "Kraj perioda")]
        public DateOnly EndDate { get; set; }
    }
}
