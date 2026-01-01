using eOdsustva.SoftverskoInzenjerstvo.Models.LeaveTypes;
using eOdsustva.SoftverskoInzenjerstvo.Models.Periods;
using System.ComponentModel.DataAnnotations;

namespace eOdsustva.SoftverskoInzenjerstvo.Models.LeaveAllocation
{
    public class LeaveAllocationListVM
    {
        public int Id { get; set; }
        public string EmployeeFullName { get; set; } = "";
        public string? DepartmentName { get; set; }
        public string LeaveTypeName { get; set; } = "";
        public string PeriodName { get; set; } = "";
        public int Days { get; set; }
    }
}
