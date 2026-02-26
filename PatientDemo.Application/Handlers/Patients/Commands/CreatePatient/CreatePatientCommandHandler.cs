using MediatR;
using PatientDemo.Application.Interfaces;
using PatientDemo.Domain.Entities;

namespace PatientDemo.Application.Handlers.Patients.Commands.CreatePatient;

/// <summary>
/// Обработчик команды создания пациента
/// </summary>
public class CreatePatientCommandHandler(IPatientDemoDbContext dbContext) : IRequestHandler<CreatePatientCommand, Guid>
{
    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            Gender = request.Gender,
            BirthDate = request.BirthDate
        };

        var humanName = new HumanName
        {
            Id = Guid.NewGuid(),
            Family = request.Family,
            Given = string.IsNullOrEmpty(request.MiddleName)
                ? [request.FirstName]
                : [request.FirstName, request.MiddleName],
            PatientId = patient.Id
        };

        await dbContext.Patients.AddAsync(patient, cancellationToken);
        await dbContext.HumanNames.AddAsync(humanName, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return patient.Id;
    }
}