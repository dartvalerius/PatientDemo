using MediatR;

using Microsoft.EntityFrameworkCore;

using PatientDemo.Application.Exceptions;
using PatientDemo.Application.Handlers.Patients.Resources;
using PatientDemo.Application.Interfaces;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Patients.Commands.DeletePatient;

/// <summary>
/// Обработчик команды удаления пациента
/// </summary>
public class DeletePatientCommandHandler(IPatientDemoDbContext dbContext) : IRequestHandler<DeletePatientCommand, Unit>
{
    public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patientEntity =
            await dbContext.Patients.SingleOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken) ??
            throw new NotFoundException(PatientResources.NotFound);

        dbContext.Patients.Remove(patientEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        request.CacheKeysToInvalidate.Add($"{nameof(PatientVm)}-{patientEntity.Id:N}");

        return default;
    }
}