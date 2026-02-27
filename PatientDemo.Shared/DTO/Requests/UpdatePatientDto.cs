using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using PatientDemo.Shared.JsonConverters;

namespace PatientDemo.Shared.DTO.Requests;

/// <summary>
/// Данные для изменения пациента
/// </summary>
public record UpdatePatientDto
{
    /// <summary>
    /// Идентификатор пациента
    /// </summary>
    [Required]
    [Description("Идентификатор пациента")]
    public Guid Id { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    [Required]
    [JsonConverter(typeof(EmptyStringToNullConverter))]
    [Description("Фамилия")]
    public string Family { get; set; } = null!;

    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    [JsonConverter(typeof(EmptyStringToNullConverter))]
    [Description("Имя")]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Отчество
    /// </summary>
    [JsonConverter(typeof(EmptyStringToNullConverter))]
    [Description("Отчество")]
    public string? MiddleName { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    [Required]
    [Description("Дата рождения")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    [Required]
    [Description("Пол")]
    [DefaultValue("Unknown")]
    public string Gender { get; set; } = "Unknown";
}