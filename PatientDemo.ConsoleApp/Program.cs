using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PatientDemo.ConsoleApp.Models;
using PatientDemo.Shared.DTO.Requests;
using PatientDemo.Shared.JsonConverters;

namespace PatientDemo.ConsoleApp;

internal class Program
{
    private static string _apiUrl = string.Empty;

    // Настройки параллельной обработки
    private static readonly int MaxParallelRequests = 20; // Увеличили до 20

    // Списки для генерации случайных данных
    private static readonly string[] LastNames =
    [
        "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецов",
        "Васильев", "Попов", "Новиков", "Федоров", "Морозов",
        "Волков", "Алексеев", "Лебедев", "Семенов", "Егоров",
        "Павлов", "Козлов", "Степанов", "Николаев", "Орлов"
    ];

    private static readonly string[] FirstNames =
    [
        "Александр", "Дмитрий", "Максим", "Сергей", "Андрей",
        "Алексей", "Артем", "Илья", "Кирилл", "Михаил",
        "Никита", "Матвей", "Роман", "Егор", "Тимофей",
        "Владимир", "Иван", "Олег", "Юрий", "Константин",
        "Анна", "Елена", "Ольга", "Наталья", "Екатерина",
        "Мария", "Татьяна", "Ирина", "Светлана", "Юлия"
    ];

    private static readonly string[] MiddleNames =
    [
        "Александрович", "Дмитриевич", "Максимович", "Сергеевич", "Андреевич",
        "Алексеевич", "Артемович", "Ильич", "Кириллович", "Михайлович",
        "Никитич", "Матвеевич", "Романович", "Егорович", "Тимофеевич",
        "Владимирович", "Иванович", "Олегович", "Юрьевич", "Константинович",
        "Александровна", "Дмитриевна", "Максимовна", "Сергеевна", "Андреевна",
        "Алексеевна", "Артемовна", "Ильинична", "Кирилловна", "Михайловна"
    ];

    private static readonly string[] Genders = { "Male", "Female" };

    // Генератор случайных чисел с потокобезопасностью
    private static readonly ThreadLocal<Random> ThreadLocalRandom = new(() => new Random(Guid.NewGuid().GetHashCode()));

    private static async Task Main()
    {
        // Проверяем, запущены ли мы в Docker
        var inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        _apiUrl = inDocker ? "http://patientdemo.webapi:80/api/Patients" : "http://localhost:5255/api/Patients";

        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Генератор пациентов (Оптимизированная версия)");
        Console.WriteLine("==============================================");
        Console.WriteLine($"API URL: {_apiUrl}");
        Console.WriteLine($"Максимально параллельных запросов: {MaxParallelRequests}");
        Console.WriteLine($"Используется SocketsHttpHandler с пулом соединений");
        Console.WriteLine();

        // Получаем оптимизированный HttpClient
        var httpClient = HttpClientFactory.Instance;

        // Генерация и отправка пациентов
        await GenerateAndSendPatientsAsync(httpClient, 100);

        Console.WriteLine();
        Console.WriteLine("Программа завершена. Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static async Task GenerateAndSendPatientsAsync(HttpClient httpClient, int count, CancellationToken cancellationToken = default)
    {
        var totalStopwatch = Stopwatch.StartNew();

        // Генерируем пациентов параллельно для ускорения
        Console.WriteLine("Генерация данных пациентов...");
        var generateStopwatch = Stopwatch.StartNew();
        var patients = await GeneratePatients(count);
        generateStopwatch.Stop();

        // Настройки параллельной обработки
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = MaxParallelRequests,
            CancellationToken = cancellationToken
        };

        var results = new ConcurrentBag<OperationResult>();

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new EmptyStringToNullConverter() }
        };

        await Parallel.ForEachAsync(patients, parallelOptions, async (patient, ct) =>
        {
            var requestStopwatch = Stopwatch.StartNew();

            try
            {
                // Создаем контент запроса
                var content = JsonContent.Create(patient, options: jsonOptions);

                // Отправляем запрос
                var response = await httpClient.PostAsync(_apiUrl, content, ct);

                requestStopwatch.Stop();

                var result = new OperationResult
                {
                    Patient = patient,
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    ElapsedMilliseconds = requestStopwatch.ElapsedMilliseconds
                };

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(ct);
                    result.ErrorMessage = errorContent.Length > 100 ? errorContent[..100] + "..." : errorContent;
                }

                results.Add(result);
            }
            catch (Exception ex)
            {
                requestStopwatch.Stop();

                var result = new OperationResult
                {
                    Patient = patient,
                    IsSuccess = false,
                    ErrorMessage = ex.Message,
                    ElapsedMilliseconds = requestStopwatch.ElapsedMilliseconds
                };

                results.Add(result);
            }
        });

        totalStopwatch.Stop();

        // Выводим финальную статистику
        Console.WriteLine("\n");
        PrintStatistics(results.ToList(), totalStopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Генератор списка данных для создания пациентов
    /// </summary>
    /// <param name="count">Количество генерируемых пациентов</param>
    /// <returns>Список данных для создания пациентов</returns>
    static Task<List<CreatePatientDto>> GeneratePatients(int count)
    {
        return Task.Run(() =>
        {
            var patients = new ConcurrentBag<CreatePatientDto>();
            var startDate = new DateTime(1950, 1, 1);
            var endDate = DateTime.Today;
            var dateRange = (endDate - startDate).Days;

            Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, _ =>
            {
                var random = ThreadLocalRandom.Value!;

                var patient = new CreatePatientDto
                {
                    Family = GetRandomItem(LastNames, random),
                    FirstName = GetRandomItem(FirstNames, random),
                    MiddleName = random.Next(3) == 0 ? null : GetRandomItem(MiddleNames, random),
                    BirthDate = startDate.AddDays(random.Next(dateRange)),
                    Gender = GetRandomItem(Genders, random)
                };

                // Корректировка пола на основе имени
                if (patient.FirstName.EndsWith("а") || patient.FirstName.EndsWith("я"))
                {
                    if (patient.FirstName != "Илья" && patient.FirstName != "Никита")
                        patient.Gender = "Female";
                    
                }
                else patient.Gender = "Male";
                

                
                patients.Add(patient);
                
            });

            return patients.ToList();
        });
    }

    private static T GetRandomItem<T>(T[] array, Random random) => array[random.Next(array.Length)];

    /// <summary>
    /// Печать статистики выполнения запросов
    /// </summary>
    /// <param name="results">Список результатов выполнения</param>
    /// <param name="totalElapsedMs">Общее время выполнения операций</param>
    private static void PrintStatistics(List<OperationResult> results, long totalElapsedMs)
    {
        var successCount = results.Count(r => r.IsSuccess);
        var errorCount = results.Count(r => !r.IsSuccess);

        Console.WriteLine("=== СТАТИСТИКА ===");
        Console.WriteLine($"Всего отправлено: {results.Count}");
        Console.WriteLine($"Успешно: {successCount}");
        Console.WriteLine($"С ошибками: {errorCount}");
        Console.WriteLine($"Общее время: {totalElapsedMs} мс ({totalElapsedMs / 1000.0:F2} сек)");

        if (!results.Any()) return;
        
        var avgTime = results.Average(r => r.ElapsedMilliseconds);
        var minTime = results.Min(r => r.ElapsedMilliseconds);
        var maxTime = results.Max(r => r.ElapsedMilliseconds);

        Console.WriteLine($"Среднее время запроса: {avgTime:F0} мс");
        Console.WriteLine($"Минимальное время: {minTime} мс");
        Console.WriteLine($"Максимальное время: {maxTime} мс");
        Console.WriteLine($"Пропускная способность: {results.Count / (totalElapsedMs / 1000.0):F1} запросов/сек");

        // Статистика по кодам ответов
        var statusGroups = results.GroupBy(r => r.StatusCode)
            .OrderBy(g => g.Key);

        Console.WriteLine("\nКоды ответов:");
        foreach (var group in statusGroups.Where(g => g.Key > 0))
        {
            Console.WriteLine($"  {group.Key}: {group.Count()} запросов");
        }
    }
}