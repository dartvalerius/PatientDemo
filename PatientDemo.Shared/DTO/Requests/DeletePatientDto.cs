using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PatientDemo.Shared.DTO.Requests;

/// <summary>
/// Данные для удаления пациента
/// </summary>
public record DeletePatientDto
{
    /// <summary>
    /// Идентификатор пациента
    /// </summary>
    [Required]
    [Description("Идентификатор пациента")]
    public Guid Id { get; set; }
}