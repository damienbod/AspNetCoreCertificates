using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureCertAuthClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Get data!");

            var json = GetApiDataUsingHttpClientHandler().GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }

        private static async Task<JsonDocument> GetApiDataUsingHttpClientHandler()
        {
            var cert = new X509Certificate2("client.pfx", "1234");
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            var client = new HttpClient(handler);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://azurecertauth20200118105901.azurewebsites.net/WeatherForecast"),
                Method = HttpMethod.Get,
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(responseContent);
                return data;
            }

            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }
    }
}
