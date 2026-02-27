using System.Globalization;
using System.Text.RegularExpressions;

using PatientDemo.Application.Interfaces;
using PatientDemo.Persistence.Resources;

namespace PatientDemo.Persistence.Parsers.Fhir.DateParser;

public class FhirDateParser : IFhirDateParser
{
    private readonly Regex PrefixRegex = new("^(eq|gt|lt|ge|le|sa|eb|ap)");
    private readonly string[] DateFormats =
    [
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-dd",
        "yyyy-MM",
        "yyyy"
    ];

    public (DateTime DateFrom, DateTime DateTo) GetPeriod(string[] dateParams)
    {
        if (dateParams == null || dateParams.Length == 0)
            return (DateTime.MinValue, DateTime.MaxValue);

        var parameters = dateParams.Select(ParseSingle).ToList();

        // Объединяем все параметры с AND-логикой
        DateTime? maxMinStart = null;  // Наибольшая нижняя граница (самое позднее начало)
        DateTime? minMaxEnd = null;    // Наименьшая верхняя граница (самый ранний конец)

        foreach (var param in parameters)
        {
            switch (param.Prefix)
            {
                // Для нижних границ (ge, gt, sa) берем максимальную
                case "ge" or "gt" or "sa" when param.FromDate.HasValue:
                {
                    if (!maxMinStart.HasValue || param.FromDate > maxMinStart)
                        maxMinStart = param.FromDate;

                    break;
                }
                // Для верхних границ (le, lt, eb) берем минимальную
                case "le" or "lt" or "eb" when param.ToDate.HasValue:
                {
                    if (!minMaxEnd.HasValue || param.ToDate < minMaxEnd)
                        minMaxEnd = param.ToDate;

                    break;
                }
                // Для eq и ap учитываем обе границы
                case "eq" or "ap":
                {
                    if (param.FromDate.HasValue)
                    {
                        if (!maxMinStart.HasValue || param.FromDate > maxMinStart)
                            maxMinStart = param.FromDate;
                    }
                    if (param.ToDate.HasValue)
                    {
                        if (!minMaxEnd.HasValue || param.ToDate < minMaxEnd)
                            minMaxEnd = param.ToDate;
                    }

                    break;
                }
            }

            minMaxEnd ??= DateTime.MaxValue;
            maxMinStart ??= DateTime.MinValue;
        }

        return (maxMinStart!.Value, minMaxEnd!.Value);
    }

    /// <summary>
    /// Парсит один параметр date
    /// </summary>
    private FhirDateParameter ParseSingle(string input)
    {
        var result = new FhirDateParameter();
        var originalInput = input;

        // 1. Извлекаем префикс
        var prefixMatch = PrefixRegex.Match(input);
        if (prefixMatch.Success)
        {
            result.Prefix = prefixMatch.Value;
            input = input.Substring(prefixMatch.Length);
        }
        else
        {
            result.Prefix = "eq"; // По умолчанию
        }

        // 2. Пробуем распарсить дату во всех возможных форматах
        foreach (var format in DateFormats)
        {
            if (DateTime.TryParseExact(input, format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var parsedDate))
            {
                // Устанавливаем границы в зависимости от формата
                SetDateBounds(result, parsedDate, format);

                // Корректируем границы согласно префиксу
                AdjustBoundsForPrefix(result);

                return result;
            }
        }

        throw new FormatException(string.Format(Messages.Fhir_UnsupportedDateFormat, originalInput));
    }

    private static void SetDateBounds(FhirDateParameter param, DateTime date, string format)
    {
        switch (format)
        {
            case "yyyy":
                // Только год: с 1 января по 31 декабря
                param.FromDate = new DateTime(date.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                param.ToDate = new DateTime(date.Year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                break;

            case "yyyy-MM":
                // Год и месяц
                param.FromDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                param.ToDate = param.FromDate.Value.AddMonths(1).AddSeconds(-1);
                break;

            case "yyyy-MM-dd":
                // Только дата
                param.FromDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                param.ToDate = param.FromDate.Value.AddDays(1).AddSeconds(-1);
                break;

            default:
                // Полная дата с временем
                param.FromDate = date;
                param.ToDate = date;
                break;
        }
    }

    private static void AdjustBoundsForPrefix(FhirDateParameter param)
    {
        switch (param.Prefix)
        {
            case "lt": // less than
            case "eb": // ends before
                param.FromDate = DateTime.MinValue;
                break;

            case "gt": // greater than
            case "sa": // starts after
                param.ToDate = DateTime.MaxValue;
                break;

            case "le": // less or equal
                       // Для le нам нужна верхняя граница, нижнюю не ограничиваем
                param.FromDate = DateTime.MinValue;
                break;

            case "ge": // greater or equal
                       // Для ge нам нужна нижняя граница, верхнюю не ограничиваем
                param.ToDate = DateTime.MaxValue;
                break;

            case "ap": // approximately
                       // Расширяем диапазон на ±1 день
                if (param.FromDate.HasValue)
                    param.FromDate = param.FromDate.Value.AddDays(-1);
                if (param.ToDate.HasValue)
                    param.ToDate = param.ToDate.Value.AddDays(1);
                break;

                // eq оставляем как есть
        }
    }
}