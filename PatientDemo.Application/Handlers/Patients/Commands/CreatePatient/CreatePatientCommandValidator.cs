using FluentValidation;
using PatientDemo.Application.Handlers.Patients.Resources;

namespace PatientDemo.Application.Handlers.Patients.Commands.CreatePatient;

/// <summary>
/// Валидатор команды создания пациента
/// </summary>
public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(c => c.Family)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PatientResources.Family_NotEmpty);

        RuleFor(c => c.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PatientResources.FirstName_NotEmpty);

        RuleFor(c => c.BirthDate)
            .Cascade(CascadeMode.Stop)
            .NotEqual((DateTime)default)
            .WithMessage(PatientResources.BirthDate_NotEmpty)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage(PatientResources.BirthDate_LessThanOrEqualToNow);

        RuleFor(c => c.Gender)
            .Cascade(CascadeMode.Stop)
            .IsInEnum()
            .WithMessage(PatientResources.Gender_IncorrectValue);
    }
}