using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace CreateSelfSignedCertsConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var dnsName = "localhost";
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            // Server self signed certificate
            var server = createClientServerAuthCerts.NewServerSelfSignedCertificate(
                new DistinguishedName { CommonName = "server", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                dnsName);

            // Client self signed certificate
            var client = createClientServerAuthCerts.NewClientSelfSignedCertificate(
                new DistinguishedName { CommonName = "client", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                dnsName);
            server.FriendlyName = "azure server certificate";
            client.FriendlyName = "azure client certificate";

            Console.WriteLine($"Created server certificate {server.FriendlyName}");

            string password = "1234";
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            var serverCertInPfxBtyes = 
                importExportCertificate.ExportSelfSignedCertificatePfx(password, server);
            File.WriteAllBytes("server.pfx", serverCertInPfxBtyes);

            var clientCertInPfxBtyes = 
                importExportCertificate.ExportSelfSignedCertificatePfx(password, client);
            File.WriteAllBytes("client.pfx", clientCertInPfxBtyes);

            var clientCertInPEMBtyes = importExportCertificate.ExportFullCertificatePem(client);
            File.WriteAllText("client.pem", clientCertInPEMBtyes);

            //var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCaL1);
            //var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
            //File.WriteAllBytes($"localhost_root_l1.cer", rootPublicKeyBytes);

            Console.WriteLine("Certificates exported to pfx and cer files");
        }
    }
}
