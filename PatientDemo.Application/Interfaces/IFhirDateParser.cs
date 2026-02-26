namespace PatientDemo.Application.Interfaces;

/// <summary>
/// Интерфейс парсера параметров даты формата FHIR
/// </summary>
public interface IFhirDateParser
{
    /// <summary>
    /// Получить период из параметров
    /// </summary>
    /// <param name="dateParams">Массив параметров даты</param>
    /// <returns>Кортеж с датами периода</returns>
    (DateTime DateFrom, DateTime DateTo) GetPeriod(string[] dateParams);
}