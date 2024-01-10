using AutoMapper;
using Core.ViewModel;
using Infrastructure.Entities; 

namespace Core.Profiles;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()  
    {
         
         CreateMap<Student, StudentListResponse>()
        .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => string.Concat(src.FirstName," ", src.LastName)))
        .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate.ToString("g")))
        .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate.ToString("g")));

        CreateMap<StudentCreateRequest, Student>()
        .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<StudentUpdateRequest, Student>()
        .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => DateTime.UtcNow));

    }
}

