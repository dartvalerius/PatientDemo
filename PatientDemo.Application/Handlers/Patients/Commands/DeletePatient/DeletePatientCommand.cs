using MediatR;

using PatientDemo.Application.Common.Cache;

namespace PatientDemo.Application.Handlers.Patients.Commands.DeletePatient;

/// <summary>
/// Команда удаления пациента
/// </summary>
public class DeletePatientCommand : IRequest<Unit>, ICacheInvalidationCommand
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    public List<string> CacheKeysToInvalidate { get; } = [];
}