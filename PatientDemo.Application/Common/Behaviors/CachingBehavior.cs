using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PatientDemo.Application.Common.Cache;

namespace PatientDemo.Application.Common.Behaviors;

/// <summary>
/// Описание поведения добавления в кэш результата запроса
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class CachingBehavior<TRequest, TResponse>(
    IMemoryCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        if (cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            logger.LogInformation($"{DateTime.Now} - Cache hit: {cacheKey}");

            return cachedResponse!;
        }

        logger.LogInformation($"{DateTime.Now} - Cache miss: {cacheKey}");

        var response = await next(cancellationToken);

        cache.Set(cacheKey, response);

        return response;
    }
}