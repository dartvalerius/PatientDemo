using MediatR;
using PatientDemo.Application.Common.Cache;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Directory.GetGenderList;

/// <summary>
/// Запрос на получение списка значений для пола пациента
/// </summary>
public class GetGenderListQuery : IRequest<IEnumerable<EnumVm>>, ICacheableQuery
{
    public string CacheKey => "GenderList";
}