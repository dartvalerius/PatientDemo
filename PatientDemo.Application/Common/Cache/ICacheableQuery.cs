namespace PatientDemo.Application.Common.Cache;

/// <summary>
/// Данные для добавления в кэш результата запроса
/// </summary>
public interface ICacheableQuery
{
    /// <summary>
    /// Ключ элемента кэша
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Время жизни записи в кэше в минутах
    /// </summary>
    int CacheDurationMinutes => 30;
}