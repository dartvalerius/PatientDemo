using FluentValidation;
using PatientDemo.Application.Handlers.Patients.Resources;

namespace PatientDemo.Application.Handlers.Patients.Commands.DeletePatient;

/// <summary>
/// Валидатор команды удаления пациента
/// </summary>
public class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
{
    public DeletePatientCommandValidator()
    {
        RuleFor(c => c.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PatientResources.Id_NotEmpty);
    }
}