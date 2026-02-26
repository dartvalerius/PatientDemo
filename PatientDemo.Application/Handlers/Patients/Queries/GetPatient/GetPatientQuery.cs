using MediatR;
using PatientDemo.Application.Common.Cache;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Patients.Queries.GetPatient;

/// <summary>
/// Запрос на получение пациента
/// </summary>
public class GetPatientQuery : IRequest<PatientVm>, ICacheableQuery
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    public string CacheKey => $"{nameof(PatientVm)}-{Id:N}";
}