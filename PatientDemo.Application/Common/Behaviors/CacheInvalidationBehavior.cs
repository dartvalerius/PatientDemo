using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using PatientDemo.Application.Common.Cache;

namespace PatientDemo.Application.Common.Behaviors;

/// <summary>
/// Описание поведения инвалидации кэша
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class CacheInvalidationBehavior<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheInvalidationCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);

        foreach (var cacheKey in request.CacheKeysToInvalidate)
        {
            cache.Remove(cacheKey);
            logger.LogInformation($"{DateTime.Now} - Cache invalidated: {cacheKey}");
        }

        return response;
    }
}