using AutoMapper;

using PatientDemo.Application.Handlers.Patients.Commands.CreatePatient;
using PatientDemo.Application.Handlers.Patients.Commands.DeletePatient;
using PatientDemo.Application.Handlers.Patients.Commands.UpdatePatient;
using PatientDemo.Domain.Enums;
using PatientDemo.Shared.DTO.Requests;

namespace PatientDemo.WebApi.Mappings;

public class PatientDTOMappingProfile : Profile
{
    public PatientDTOMappingProfile()
    {
        AllowNullCollections = true;

        CreateMap<CreatePatientDto, CreatePatientCommand>()
            .ForMember(x => x.Family, options => options.MapFrom(x => x.Family))
            .ForMember(x => x.FirstName, options => options.MapFrom(x => x.FirstName))
            .ForMember(x => x.MiddleName, options => options.MapFrom(x => x.MiddleName))
            .ForMember(x => x.BirthDate, options => options.MapFrom(x => x.BirthDate))
            .ForMember(x => x.Gender, options => options.MapFrom(x => MapGender(x.Gender)));

        CreateMap<UpdatePatientDto, UpdatePatientCommand>()
            .ForMember(x => x.Id, options => options.MapFrom(x => x.Id))
            .ForMember(x => x.Family, options => options.MapFrom(x => x.Family))
            .ForMember(x => x.FirstName, options => options.MapFrom(x => x.FirstName))
            .ForMember(x => x.MiddleName, options => options.MapFrom(x => x.MiddleName))
            .ForMember(x => x.BirthDate, options => options.MapFrom(x => x.BirthDate))
            .ForMember(x => x.Gender, options => options.MapFrom(x => MapGender(x.Gender)));

        CreateMap<DeletePatientDto, DeletePatientCommand>()
            .ForMember(x => x.Id, options => options.MapFrom(x => x.Id));
    }

    private static Gender MapGender(string genderString)
    {
        return Enum.TryParse<Gender>(genderString, true, out var gender)
            ? gender
            : Gender.Unknown;
    }
}