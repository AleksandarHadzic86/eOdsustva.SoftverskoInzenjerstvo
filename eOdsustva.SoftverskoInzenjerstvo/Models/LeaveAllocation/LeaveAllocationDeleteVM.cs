namespace eOdsustva.SoftverskoInzenjerstvo.Models.LeaveAllocation
{
    public class LeaveAllocationDeleteVM:LeaveAllocationListVM
    {
        public string EmployeeId { get; set; } = "";
        public int LeaveTypeId { get; set; }
        public int PeriodId { get; set; }
    }
}
