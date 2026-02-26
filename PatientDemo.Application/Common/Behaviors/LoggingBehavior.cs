using MediatR;
using Microsoft.Extensions.Logging;

namespace PatientDemo.Application.Common.Behaviors;

/// <summary>
/// Описание поведения логирования
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation($"Handling {requestName}");

        var response = await next(cancellationToken);

        logger.LogInformation($"Handled {requestName}");

        return response;
    }
}