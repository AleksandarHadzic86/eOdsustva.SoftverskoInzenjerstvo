using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Models.LeaveTypes
{
    public class LeaveTypeEditVM : BaseLeaveTypeVM
    {
        [Required]
        [Length(3, 100, ErrorMessage = "Prekoracili ste duzinu karaktera")]
        [Display(Name = "Naziv")]
        public string Name { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "Broj dana mora biti između 1 i 10.")]
        [Display(Name = "Broj dana")]
        public int NumberOfDays { get; set; }
    }
}
