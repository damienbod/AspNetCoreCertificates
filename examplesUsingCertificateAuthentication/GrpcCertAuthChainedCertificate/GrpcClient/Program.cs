using Grpc.Core;
using Grpc.Net.Client;
using GrpcCertAuthChainedCertificate;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var handler = new HttpClientHandler();
            var certificate = new X509Certificate2("clientl4.pfx", "1234");
            handler.ClientCertificates.Add(certificate);

            var channel = GrpcChannel.ForAddress("https://localhost:44379", new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler)
            });

            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
