namespace PatientDemo.Persistence.Parsers.Fhir.DateParser;

/// <summary>
/// Результат парсинга одного из параметров даты
/// </summary>
class FhirDateParameter
{
    public string? Prefix { get; set; } // eq, gt, lt, ge, le, sa, eb, ap, ne
    public DateTime? FromDate { get; set; } // Нижняя граница
    public DateTime? ToDate { get; set; }   // Верхняя граница
}