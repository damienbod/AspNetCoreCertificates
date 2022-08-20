using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreCertificateAuth.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task OnGetAsync()
    {
        var chainedClient = await CallApiClientChainedClientCertLocalhost();
    }

    private async Task<JsonDocument> CallApiClientChainedClientCertLocalhost()
    {
        try
        {
            // chained client from an intermediate - root
            var client = _clientFactory.CreateClient("chained_client");

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://localhost:44378/api/values"),
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
        catch (Exception e)
        {
            throw new ApplicationException($"Exception {e}");
        }
    }
}
