using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PatientDemo.Shared.DTO.Responses;

/// <summary>
/// Модель отображения имени пациента
/// </summary>
public class HumanNameVm
{
    /// <summary>
    /// Идентификатор пациента
    /// </summary>
    [JsonRequired]
    [JsonPropertyOrder(0)]
    [Description("Идентификатор пациента")]
    public Guid Id { get; set; }

    /// <summary>
    /// Тип имени пациента (official - актуальное имя пациента)
    /// Нужно делать перечислением, но пока как заглушка
    /// </summary>
    [JsonPropertyOrder(1)]
    [Description("Тип имени пациента")]
    public string Use { get; set; } = "official";

    /// <summary>
    /// Фамилия
    /// </summary>
    [JsonRequired]
    [JsonPropertyOrder(2)]
    [Description("Фамилия")]
    public required string Family { get; set; }

    /// <summary>
    /// Имя и отчество
    /// </summary>
    [JsonPropertyOrder(3)]
    [Description("Имя и отчество")]
    public string[] Given { get; set; } = [];
}