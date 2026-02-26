using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PatientDemo.Shared.DTO.Responses;

/// <summary>
/// Модель отображения данных пациента
/// </summary>
public class PatientVm
{
    /// <summary>
    /// Имя пациента
    /// </summary>
    [JsonPropertyOrder(0)]
    [Description("Имя пациента")]
    public HumanNameVm Name { get; set; } = null!;

    /// <summary>
    /// Пол
    /// </summary>
    [JsonPropertyOrder(1)]
    [Description("Пол")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Дата рождения
    /// </summary>
    [JsonRequired]
    [JsonPropertyOrder(2)]
    [Description("Пол")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Указывает статус ресурса: активен или нет.
    /// Активные ресурсы доступны для редактирования и для импорта связанных с ним ресурсов.
    /// При импорте сведений о пациенте возможно указывать в элементе `Patient.active` только значение `true`.
    /// </summary>
    [JsonPropertyOrder(3)]
    [Description("Статус ресурса")]
    public bool Active { get; set; } = true;
}