using PatientDemo.Domain.Enums;

namespace PatientDemo.Domain.Entities;

/// <summary>
/// Пациент
/// </summary>
public class Patient
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Пол
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Указывает статус ресурса: активен или нет.
    /// Активные ресурсы доступны для редактирования и для импорта связанных с ним ресурсов.
    /// При импорте сведений о пациенте возможно указывать в элементе `Patient.active` только значение `true`.
    /// </summary>
    public bool Active { get; set; }
}