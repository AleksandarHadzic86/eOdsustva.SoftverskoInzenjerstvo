using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Models.LeaveAllocation
{
    public class LeaveAllocationCreateVM
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = "";

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public int PeriodId { get; set; }

        [Range(1, 365)]
        public int Days { get; set; }
    }
}
