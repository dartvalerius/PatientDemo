using MediatR;

using PatientDemo.Application.Common.Cache;
using PatientDemo.Domain.Enums;

namespace PatientDemo.Application.Handlers.Patients.Commands.UpdatePatient;

/// <summary>
/// Команда обновления данных пациента
/// </summary>
public class UpdatePatientCommand : IRequest<Unit>, ICacheInvalidationCommand
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public string Family { get; set; } = null!;

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    public Gender Gender { get; set; }

    public List<string> CacheKeysToInvalidate { get; } = [];
}