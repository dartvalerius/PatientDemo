namespace PatientDemo.Application.Common.Cache;

/// <summary>
/// Данные для инвалидации кэша по результату команды
/// </summary>
public interface ICacheInvalidationCommand
{
    /// <summary>
    /// Ключи для инвалидации кэша
    /// </summary>
    List<string> CacheKeysToInvalidate { get; }
}