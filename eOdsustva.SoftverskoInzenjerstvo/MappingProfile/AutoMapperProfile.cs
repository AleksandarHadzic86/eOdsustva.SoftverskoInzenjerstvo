using AutoMapper;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using eOdsustva.SoftverskoInzenjerstvo.Models.LeaveTypes;


namespace eOdsustva.SoftverskoInzenjerstvo.MappingProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LeaveType, LeaveTypeReadOnlyVM>();
            CreateMap<LeaveTypeCreateVM, LeaveType>().ForMember(d => d.Id, opt => opt.Ignore());
            CreateMap<LeaveTypeEditVM, LeaveType>().ReverseMap();
        }
    }
}
