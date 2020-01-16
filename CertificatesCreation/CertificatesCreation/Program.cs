using Microsoft.Extensions.DependencyInjection;
using System;
using CertificateManager;
using CertificateManager.Models;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace CertificatesCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            //LowLevelApiExamples.Run();

            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();

            var rootCaL1 = certManagerService.CreateRootCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "root dev", Country = "IT" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            rootCaL1.FriendlyName = "developement root L1 certificate";

            // Intermediate L2 chained from root L1
            var intermediateCaL2 = certManagerService.CreateIntermediateCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                2,  "localhost", rootCaL1);
            intermediateCaL2.FriendlyName = "developement Intermediate L2 certificate";

            // Server, Client L3 chained from Intermediate L2
            var serverL3 = certManagerService.CreateServerCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "server", Country = "DE" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);

            var clientL3 = certManagerService.CreateClientCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "client", Country = "IE" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);
            serverL3.FriendlyName = "developement server L3 certificate";
            clientL3.FriendlyName = "developement client L3 certificate";
            
            Console.WriteLine($"Created Client, Server L3 Certificates {clientL3.FriendlyName}");

            string password = "1234";
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, rootCaL1);
            File.WriteAllBytes("localhost_root_l1.pfx", rootCertInPfxBtyes);

            var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCaL1);
            var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"localhost_root_l1.cer", rootPublicKeyBytes);

            var intermediateCertInPfxBtyes = importExportCertificate.ExportCertificatePfx(password, intermediateCaL2, rootCaL1);
            File.WriteAllBytes("localhost_intermediate_l2.pfx", intermediateCertInPfxBtyes);

            var serverCertL3InPfxBtyes = importExportCertificate.ExportCertificatePfx(password, serverL3, intermediateCaL2);
            File.WriteAllBytes("serverl3.pfx", serverCertL3InPfxBtyes);

            var clientCertL3InPfxBtyes = importExportCertificate.ExportCertificatePfx(password, clientL3, intermediateCaL2);
            File.WriteAllBytes("clientl3.pfx", clientCertL3InPfxBtyes);
        }
    }
}
