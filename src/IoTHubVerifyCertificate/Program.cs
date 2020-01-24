using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace IoTHubVerifyCertificate
{

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-x509-get-started
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            var root = new X509Certificate2("root.pfx", "1234");

            var deviceVerify = createClientServerAuthCerts.NewDeviceVerificationCertificate(
            "4C8C754C6DA4280DBAB7FC7BB320E7FFFB7F411CBB7EAA7D", root);
            deviceVerify.FriendlyName = "device verify";

            var deviceVerifyPEM = importExportCertificate.PemExportPublicKeyCertificate(deviceVerify);
            File.WriteAllText("deviceVerify.pem", deviceVerifyPEM);

            var deviceVerifyPublicKey = importExportCertificate.ExportCertificatePublicKey(deviceVerify);
            var deviceVerifyPublicKeyBytes = deviceVerifyPublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"deviceVerify.cer", deviceVerifyPublicKeyBytes);

            Console.WriteLine("Certificates exported to pfx and cer files");
        }
    }
}