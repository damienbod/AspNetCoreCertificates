using Grpc.Core;
using Grpc.Net.Client;
using GrpcCertAuthChainedCertificate;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
            {
                HttpClient = CreateHttpClient()
            });

            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            var cert = new X509Certificate2("clientl4.pfx", "1234");
            handler.ClientCertificates.Add(cert);

            // Create client
            return new HttpClient(handler);
        }
    }
}
