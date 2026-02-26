using AutoMapper;
using PatientDemo.Domain.Entities;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Common.Mappings;

public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Patient, HumanNameVm>()
            .ForMember(x => x.Id, options => options.MapFrom(x => x.Id))
            .ForMember(x => x.Use, options => options.MapFrom(x => x.Name!.Use))
            .ForMember(x => x.Family, options => options.MapFrom(x => x.Name!.Family))
            .ForMember(x => x.Given, options => options.MapFrom(x => x.Name!.Given));

        CreateMap<Patient, PatientVm>()
            .ForMember(x => x.Name, options => options.MapFrom(x => x))
            .ForMember(x => x.BirthDate, options => options.MapFrom(x => x.BirthDate))
            .ForMember(x => x.Gender, options => options.MapFrom(x => x.Gender.ToString()))
            .ForMember(x => x.Active, options => options.MapFrom(x => x.Active));
    }
}