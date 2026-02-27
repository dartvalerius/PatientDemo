using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using PatientDemo.Shared.DTO.Requests;
using PatientDemo.Shared.JsonConverters;

namespace PatientDemo.ConsoleApp
{
    // Класс для хранения результата операции
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public int Index { get; set; }
        public CreatePatientDto? Patient { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }

    public static class HttpClientFactory
    {
        private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() =>
        {
            // Настраиваем обработчик для оптимальной работы с HTTP
            var handler = new SocketsHttpHandler
            {
                // Увеличиваем лимит соединений
                MaxConnectionsPerServer = 20,

                // Настройки таймаутов
                ConnectTimeout = TimeSpan.FromSeconds(10),
                ResponseDrainTimeout = TimeSpan.FromSeconds(5),

                // Включаем автоматическую декомпрессию
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,

                // Оптимизация для keep-alive соединений
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),

                // Включаем поддержку HTTP/2
                EnableMultipleHttp2Connections = true,

                // Оптимизация для параллельных запросов
                Expect100ContinueTimeout = TimeSpan.FromSeconds(2)
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Настройка заголовков по умолчанию
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "PatientGenerator/2.0");
            client.DefaultRequestHeaders.ConnectionClose = false; // Используем keep-alive

            return client;
        });

        public static HttpClient Instance => _httpClient.Value;
    }

    class Program
    {
        private static readonly string apiUrl = "http://localhost:5255/api/Patients";

        // Настройки параллельной обработки
        private static readonly int maxParallelRequests = 20; // Увеличили до 20
        private static readonly int requestTimeoutMs = 30000;

        // Списки для генерации случайных данных
        private static readonly string[] lastNames =
        {
            "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецов",
            "Васильев", "Попов", "Новиков", "Федоров", "Морозов",
            "Волков", "Алексеев", "Лебедев", "Семенов", "Егоров",
            "Павлов", "Козлов", "Степанов", "Николаев", "Орлов"
        };

        private static readonly string[] firstNames =
        {
            "Александр", "Дмитрий", "Максим", "Сергей", "Андрей",
            "Алексей", "Артем", "Илья", "Кирилл", "Михаил",
            "Никита", "Матвей", "Роман", "Егор", "Тимофей",
            "Владимир", "Иван", "Олег", "Юрий", "Константин",
            "Анна", "Елена", "Ольга", "Наталья", "Екатерина",
            "Мария", "Татьяна", "Ирина", "Светлана", "Юлия"
        };

        private static readonly string[] middleNames =
        {
            "Александрович", "Дмитриевич", "Максимович", "Сергеевич", "Андреевич",
            "Алексеевич", "Артемович", "Ильич", "Кириллович", "Михайлович",
            "Никитич", "Матвеевич", "Романович", "Егорович", "Тимофеевич",
            "Владимирович", "Иванович", "Олегович", "Юрьевич", "Константинович",
            "Александровна", "Дмитриевна", "Максимовна", "Сергеевна", "Андреевна",
            "Алексеевна", "Артемовна", "Ильинична", "Кирилловна", "Михайловна"
        };

        private static readonly string[] genders = { "Male", "Female" };

        // Генератор случайных чисел с потокобезопасностью
        private static readonly ThreadLocal<Random> threadLocalRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Генератор пациентов (Оптимизированная версия)");
            Console.WriteLine("==============================================");
            Console.WriteLine($"API URL: {apiUrl}");
            Console.WriteLine($"Максимально параллельных запросов: {maxParallelRequests}");
            Console.WriteLine($"Используется SocketsHttpHandler с пулом соединений");
            Console.WriteLine();

            // Получаем оптимизированный HttpClient
            var httpClient = HttpClientFactory.Instance;

            // Генерация и отправка пациентов
            await GenerateAndSendPatientsParallel(httpClient, 10000);

            Console.WriteLine();
            Console.WriteLine("Программа завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static async Task GenerateAndSendPatientsParallel(HttpClient httpClient, int count)
        {
            var totalStopwatch = Stopwatch.StartNew();

            // Генерируем пациентов параллельно для ускорения
            Console.WriteLine("Генерация данных пациентов...");
            var generateStopwatch = Stopwatch.StartNew();
            var patients = await GeneratePatientsParallel(count);
            generateStopwatch.Stop();

            Console.WriteLine($"Сгенерировано {patients.Count} пациентов за {generateStopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine("Начинаем параллельную отправку запросов...");
            Console.WriteLine();

            // Настройки параллельной обработки
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelRequests
            };

            // Используем Concurrent коллекции для потокобезопасности
            var results = new System.Collections.Concurrent.ConcurrentBag<OperationResult>();
            var processedCount = 0;
            var totalPatients = patients.Count;
            var lastProgressUpdate = 0;

            // Создаем сериализатор для JSON
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new EmptyStringToNullConverter() }
            };

            // Используем семафор для ограничения параллельных запросов (дополнительная защита)
            using var semaphore = new SemaphoreSlim(maxParallelRequests);

            // Создаем задачи для всех пациентов
            var tasks = patients.Select(async (patient, index) =>
            {
                await semaphore.WaitAsync();
                var requestStopwatch = Stopwatch.StartNew();

                try
                {
                    // Создаем контент запроса
                    var content = JsonContent.Create(patient, options: jsonOptions);

                    // Отправляем запрос
                    var response = await httpClient.PostAsync(apiUrl, content);

                    requestStopwatch.Stop();

                    var result = new OperationResult
                    {
                        Index = index + 1,
                        Patient = patient,
                        IsSuccess = response.IsSuccessStatusCode,
                        StatusCode = (int)response.StatusCode,
                        ElapsedMilliseconds = requestStopwatch.ElapsedMilliseconds
                    };

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        result.ErrorMessage = errorContent.Length > 100 ? errorContent[..100] + "..." : errorContent;
                    }

                    results.Add(result);

                    // Обновляем прогресс
                    var currentProcessed = Interlocked.Increment(ref processedCount);
                    var progress = (currentProcessed * 100) / totalPatients;

                    if (progress > lastProgressUpdate && progress % 5 == 0)
                    {
                        lastProgressUpdate = progress;
                        var successCount = results.Count(r => r.IsSuccess);
                        var errorCount = results.Count(r => !r.IsSuccess);

                        lock (Console.Out)
                        {
                            var originalColor = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($"\rПрогресс: {progress}% ({currentProcessed}/{totalPatients}) ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"✓ {successCount} ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"✗ {errorCount}");
                            Console.ForegroundColor = originalColor;
                        }
                    }

                    // Логируем результат (каждый 5-й или ошибки)
                    if (!result.IsSuccess || index % 5 == 0)
                    {
                        var statusSymbol = result.IsSuccess ? "✓" : "✗";
                        var statusColor = result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red;

                        lock (Console.Out)
                        {
                            Console.WriteLine(); // Новая строка после прогресса
                            var originalColor = Console.ForegroundColor;
                            Console.ForegroundColor = statusColor;
                            Console.Write($"{statusSymbol} ");
                            Console.ForegroundColor = originalColor;
                            Console.WriteLine($"Пациент {index + 1}: {patient.Family} {patient.FirstName} " +
                                            $"- Status: {result.StatusCode} ({result.ElapsedMilliseconds} мс)");
                        }
                    }
                }
                catch (Exception ex)
                {
                    requestStopwatch.Stop();

                    var result = new OperationResult
                    {
                        Index = index + 1,
                        Patient = patient,
                        IsSuccess = false,
                        ErrorMessage = ex.Message,
                        ElapsedMilliseconds = requestStopwatch.ElapsedMilliseconds
                    };

                    results.Add(result);
                    Interlocked.Increment(ref processedCount);

                    lock (Console.Out)
                    {
                        var originalColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("✗ ");
                        Console.ForegroundColor = originalColor;
                        Console.WriteLine($"Пациент {index + 1}: {patient.Family} {patient.FirstName} " +
                                        $"- Ошибка: {ex.Message} ({result.ElapsedMilliseconds} мс)");
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            totalStopwatch.Stop();

            // Выводим финальную статистику
            Console.WriteLine("\n");
            PrintStatistics(results.ToList(), totalStopwatch.ElapsedMilliseconds);
        }

        static Task<List<CreatePatientDto>> GeneratePatientsParallel(int count)
        {
            return Task.Run(() =>
            {
                var patients = new List<CreatePatientDto>(count);
                var startDate = new DateTime(1950, 1, 1);
                var endDate = DateTime.Today;
                var dateRange = (endDate - startDate).Days;

                // Используем Parallel.For для ускорения генерации
                var lockObject = new object();

                Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
                {
                    var random = threadLocalRandom.Value!;

                    var patient = new CreatePatientDto
                    {
                        Family = GetRandomItem(lastNames, random),
                        FirstName = GetRandomItem(firstNames, random),
                        MiddleName = random.Next(3) == 0 ? null : GetRandomItem(middleNames, random),
                        BirthDate = startDate.AddDays(random.Next(dateRange)),
                        Gender = GetRandomItem(genders, random)
                    };

                    // Корректировка пола на основе имени
                    if (patient.FirstName.EndsWith("а") || patient.FirstName.EndsWith("я"))
                    {
                        if (patient.FirstName != "Илья" && patient.FirstName != "Никита")
                        {
                            patient.Gender = "Female";
                        }
                    }
                    else
                    {
                        patient.Gender = "Male";
                    }

                    lock (lockObject)
                    {
                        patients.Add(patient);
                    }
                });

                return patients;
            });
        }

        static T GetRandomItem<T>(T[] array, Random random)
        {
            return array[random.Next(array.Length)];
        }

        static void PrintStatistics(List<OperationResult> results, long totalElapsedMs)
        {
            var successCount = results.Count(r => r.IsSuccess);
            var errorCount = results.Count(r => !r.IsSuccess);

            Console.WriteLine("=== СТАТИСТИКА ===");
            Console.WriteLine($"Всего отправлено: {results.Count}");
            Console.WriteLine($"Успешно: {successCount}");
            Console.WriteLine($"С ошибками: {errorCount}");
            Console.WriteLine($"Общее время: {totalElapsedMs} мс ({totalElapsedMs / 1000.0:F2} сек)");

            if (results.Count > 0)
            {
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

                // Если были ошибки, показываем топ-3
                if (errorCount > 0)
                {
                    Console.WriteLine("\nПримеры ошибок:");
                    foreach (var error in results.Where(r => !r.IsSuccess).Take(3))
                    {
                        Console.WriteLine($"  Пациент {error.Index}: {error.ErrorMessage}");
                    }
                }
            }
        }
    }
}