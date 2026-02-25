using PatientDemo.Domain.Attributes;
using PatientDemo.Domain.Resources;

namespace PatientDemo.Domain.Enums;

/// <summary>
/// Пол
/// </summary>
public enum Gender
{
    /// <summary>
    /// Неизвестный
    /// </summary>
    [LocalizedDescription("Unknown", typeof(GenderLocalizedDescription))]
    Unknown,

    /// <summary>
    /// Мужской
    /// </summary>
    [LocalizedDescription("Male", typeof(GenderLocalizedDescription))]
    Male,

    /// <summary>
    /// Женский
    /// </summary>
    [LocalizedDescription("Female", typeof(GenderLocalizedDescription))]
    Female,

    /// <summary>
    /// Другой
    /// </summary>
    [LocalizedDescription("Other", typeof(GenderLocalizedDescription))]
    Other,
}