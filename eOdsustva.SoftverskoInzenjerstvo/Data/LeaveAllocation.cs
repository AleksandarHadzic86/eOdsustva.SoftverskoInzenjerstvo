using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Data
{
    public class LeaveAllocation : BaseEntity
    {
        [Required]
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; } = null;


        public string EmployeeId { get; set; } = string.Empty;
        public ApplicationUser? Employee { get; set; } = null;

        [Required]
        public int PeriodId { get; set; }
        public Period? Period { get; set; } = null;

        [Range(1, 365)]
        public int Days { get; set; }


    }
}
