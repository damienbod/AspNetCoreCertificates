using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace IoTHubCreateChainedCerts
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

            var root = createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName { CommonName = "root dev", Country = "IT" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            root.FriendlyName = "developement root certificate";

            // Intermediate L2 chained from root L1
            var intermediate = createClientServerAuthCerts.NewIntermediateChainedCertificate(
                new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                2, "localhost", root);
            intermediate.FriendlyName = "developement Intermediate certificate";

            string password = "1234";
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, root);
            File.WriteAllBytes("root.pfx", rootCertInPfxBtyes);

            // https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-x509-get-started

            var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(root);
            var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"root.cer", rootPublicKeyBytes);

            var intermediateCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, intermediate, root);
            File.WriteAllBytes("intermediate.pfx", intermediateCertInPfxBtyes);

            Console.WriteLine("Certificates exported to pfx and cer files");
        }
    }
}
