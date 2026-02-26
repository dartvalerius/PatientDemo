using FluentValidation;
using MediatR;

namespace PatientDemo.Application.Common.Behaviors;

/// <summary>
/// Описание поведения валидатора
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
/// <param name="validators">Список валидаторов</param>
public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(async v => await v.ValidateAsync(validationContext, cancellationToken))
            .SelectMany(result => result.Result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Any()) throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}