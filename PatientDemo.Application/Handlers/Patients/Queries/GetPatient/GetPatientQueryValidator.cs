using FluentValidation;
using PatientDemo.Application.Handlers.Patients.Resources;

namespace PatientDemo.Application.Handlers.Patients.Queries.GetPatient;

/// <summary>
/// Валидатор запроса на получение пациента
/// </summary>
public class GetPatientQueryValidator : AbstractValidator<GetPatientQuery>
{
    public GetPatientQueryValidator()
    {
        RuleFor(q => q.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(PatientResources.Id_NotEmpty);
    }
}