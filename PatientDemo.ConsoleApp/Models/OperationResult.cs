using PatientDemo.Shared.DTO.Requests;

namespace PatientDemo.ConsoleApp.Models;

/// <summary>
/// Класс для хранения результата операции
/// </summary>
public class OperationResult
{
    /// <summary>
    /// Флаг успешного завершения операции
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Данные для создания пациента
    /// </summary>
    public CreatePatientDto? Patient { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Код ответа выполненного запроса
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Продолжительность операции
    /// </summary>
    public long ElapsedMilliseconds { get; set; }
}