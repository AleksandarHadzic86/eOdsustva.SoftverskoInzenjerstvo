using AutoMapper;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using eOdsustva.SoftverskoInzenjerstvo.Models.LeaveAllocation;
using eOdsustva.SoftverskoInzenjerstvo.Models.LeaveTypes;
using eOdsustva.SoftverskoInzenjerstvo.Models.Periods;


namespace eOdsustva.SoftverskoInzenjerstvo.MappingProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LeaveType, LeaveTypeReadOnlyVM>();
            CreateMap<LeaveTypeCreateVM, LeaveType>().ForMember(d => d.Id, opt => opt.Ignore());
            CreateMap<LeaveTypeEditVM, LeaveType>().ReverseMap();

            CreateMap<Period, PeriodVM>();

      
            CreateMap<LeaveAllocation, LeaveAllocationListVM>()
                .ForMember(d => d.EmployeeFullName,
                    o => o.MapFrom(s => s.Employee.FirstName + " " + s.Employee.LastName))
                .ForMember(d => d.DepartmentName,
                    o => o.MapFrom(s => s.Employee.Department.Name))
                .ForMember(d => d.LeaveTypeName,
                    o => o.MapFrom(s => s.LeaveType.Name))
                .ForMember(d => d.PeriodName,
                    o => o.MapFrom(s => s.Period.Name));

            CreateMap<LeaveAllocation, LeaveAllocationDetailsVM>()
                .IncludeBase<LeaveAllocation, LeaveAllocationListVM>();

            CreateMap<LeaveAllocation, LeaveAllocationEditVM>().ReverseMap();

            CreateMap<LeaveAllocation, LeaveAllocationCreateVM>().ReverseMap();

            CreateMap<LeaveAllocation, LeaveAllocationDeleteVM>()
            .IncludeBase<LeaveAllocation, LeaveAllocationListVM>();

        }
    }
}
