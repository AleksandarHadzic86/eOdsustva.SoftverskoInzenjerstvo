using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Models.LeaveTypes
{
    public class LeaveTypeReadOnlyVM : BaseLeaveTypeVM
    {
        
        [Display(Name = "Naziv")]
        public string Name { get; set; }


        [Display(Name = "Broj dana")]
        public int NumberOfDays { get; set; }

    }
}
