using System.Net;

namespace PatientDemo.ConsoleApp;

/// <summary>
/// Класс для получения оптимально настроенного http клиента
/// </summary>
public static class HttpClientFactory
{
    private static readonly Lazy<HttpClient> HttpClient = new(() =>
    {
        var handler = new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 20,

            ConnectTimeout = TimeSpan.FromSeconds(10),
            ResponseDrainTimeout = TimeSpan.FromSeconds(5),
            Expect100ContinueTimeout = TimeSpan.FromSeconds(2),

            AutomaticDecompression = DecompressionMethods.All,

            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            KeepAlivePingDelay = TimeSpan.FromSeconds(30),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,

            EnableMultipleHttp2Connections = true,

            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30),
            DefaultRequestVersion = HttpVersion.Version20,
        };

        // Заголовки
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent", "PatientGenerator/2.0");
        client.DefaultRequestHeaders.ConnectionClose = false;

        return client;
    });

    public static HttpClient Instance => HttpClient.Value;
}