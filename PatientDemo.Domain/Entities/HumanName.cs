namespace PatientDemo.Domain.Entities;

/// <summary>
/// Имя
/// </summary>
public class HumanName
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип имени пациента (official - актуальное имя пациента)
    /// </summary>
    public string Use { get; set; } = "official";

    /// <summary>
    /// Фамилия
    /// </summary>
    public string Family { get; set; } = null!;

    /// <summary>
    /// Имя и отчество
    /// </summary>
    public string Given { get; set; } = null!;
}