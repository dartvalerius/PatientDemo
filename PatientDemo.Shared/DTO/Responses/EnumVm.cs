namespace PatientDemo.Shared.DTO.Responses;

/// <summary>
/// Модель отображения перечисления
/// </summary>
public class EnumVm
{
    /// <summary>
    /// Значение
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public required string Description { get; set; }
}