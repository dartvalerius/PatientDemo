using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientDemo.Application.Exceptions;
using PatientDemo.Application.Handlers.Patients.Resources;
using PatientDemo.Application.Interfaces;
using PatientDemo.Shared.DTO.Responses;

namespace PatientDemo.Application.Handlers.Patients.Commands.UpdatePatient;

/// <summary>
/// Обработчик команды обновления данных пациента
/// </summary>
public class UpdatePatientCommandHandler(IPatientDemoDbContext dbContext) : IRequestHandler<UpdatePatientCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patientEntity =
            await dbContext.Patients.Include(x => x.Name)
                .SingleOrDefaultAsync(x => x.Id.Equals(request.Id), cancellationToken) ??
            throw new NotFoundException(PatientResources.NotFound);

        patientEntity.BirthDate = request.BirthDate;
        patientEntity.Gender = request.Gender;
        patientEntity.Name!.Family = request.Family;
        patientEntity.Name!.Given = string.IsNullOrEmpty(request.MiddleName)
            ? [request.FirstName]
            : [request.FirstName, request.MiddleName];

        await dbContext.SaveChangesAsync(cancellationToken);

        request.CacheKeysToInvalidate.Add($"{nameof(PatientVm)}-{patientEntity.Id:N}");

        return default;
    }
}