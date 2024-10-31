using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace AzureCertAuthClientConsole;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Get data!");

        var json = await GetApiDataUsingHttpClientHandler();

        Console.WriteLine("Success!");
        Console.ReadLine();
    }

    private static async Task<JsonDocument> GetApiDataUsingHttpClientHandler()
    {
        var cert = new X509Certificate2("client.pfx", "1234");
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(cert);
        var client = new HttpClient(handler);

        var url = "https://localhost:44361/WeatherForecast";
        //var url = "https://azurecertauth20201108214641.azurewebsites.net/WeatherForecast";
        var request = new HttpRequestMessage()
        {     
            RequestUri = new Uri(url),
            Method = HttpMethod.Get,
        };
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var data = JsonDocument.Parse(responseContent);
            return data;
        }

        throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
    }
}
