using MediatR;
using PatientDemo.Domain.Enums;

namespace PatientDemo.Application.Handlers.Patients.Commands.CreatePatient;

/// <summary>
/// Команда создания пациента
/// </summary>
public class CreatePatientCommand : IRequest<Guid>
{
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
}