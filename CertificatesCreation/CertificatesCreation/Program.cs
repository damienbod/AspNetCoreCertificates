using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CertificateManager;
using System.Security.Cryptography;
using CertificateManager.Models;

namespace CertificatesCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            //LowLevelApis.Run();

            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();

            var rootCaL1 = certManagerService.CreateRootCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "localhost", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");

            // Intermediate L2 chained from root L1
            var intermediateCaL2 = certManagerService.CreateIntermediateCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "localhost", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                2,  "localhost", rootCaL1);
            intermediateCaL2.FriendlyName = "developement Intermediate L2 certificate";

            // Server, Client L3 chained from Intermediate L2
            var serverL3 = certManagerService.CreateServerCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "localhost", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);

            var clientL3 = certManagerService.CreateClientCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "localhost", Country = "CH" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);
            serverL3.FriendlyName = "developement server L3 certificate";
            clientL3.FriendlyName = "developement client L3 certificate";
            
            Console.WriteLine($"Created Client, Server L3 Certificates {clientL3.FriendlyName}");
        }
    }
}
