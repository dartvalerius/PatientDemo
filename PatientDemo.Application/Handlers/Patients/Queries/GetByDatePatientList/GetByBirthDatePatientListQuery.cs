using MediatR;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Patients.Queries.GetByDatePatientList;

/// <summary>
/// Запрос на получение пациентов по дате рождения
/// Для демонстрации работы с переданными параметрами даты date=ge2010-01-01&date=le2011-12-31
/// </summary>
public class GetByBirthDatePatientListQuery : IRequest<IEnumerable<PatientVm>>
{
    /// <summary>
    /// Дата рождения с
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Дата рождения по
    /// </summary>
    public DateTime? DateTo { get; set; }
}