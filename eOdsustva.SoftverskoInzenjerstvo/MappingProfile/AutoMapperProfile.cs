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


        }
    }
}
