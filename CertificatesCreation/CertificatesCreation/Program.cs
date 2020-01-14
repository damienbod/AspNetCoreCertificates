using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using CertificateManager;
namespace CertificatesCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create Root Certificate");

            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var rcCreator = serviceProvider.GetService<RootCertificate>();

            var rootCert = rcCreator.CreateRootCertificate(
                RootCertConfig.DistinguishedName,
                RootCertConfig.BasicConstraints,
                RootCertConfig.ValidityPeriod,
                RootCertConfig.SubjectAlternativeName);

            Console.WriteLine($"Created Root Certificate {rootCert.SubjectName}");


            var icCreator = serviceProvider.GetService<IntermediateCertificate>();

            var intermediateCertificate = icCreator.CreateIntermediateCertificate(
                IntermediateCertConfig.DistinguishedName,
                IntermediateCertConfig.BasicConstraints,
                IntermediateCertConfig.ValidityPeriod,
                IntermediateCertConfig.SubjectAlternativeName,
                rootCert);

            Console.WriteLine($"Created Intermediate Certificate {intermediateCertificate.SubjectName}");

            string password = "1234";
            string rootCertName = "localhost_root";
            string intermediateCertName = "localhost_intermediate";

            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            importExportCertificate.SaveCertificateToPfxFile($"{rootCertName}.pfx", password, rootCert, null, null);
            var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCert);
            var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"{rootCertName}.cer", rootPublicKeyBytes);

            var chain = new X509Certificate2Collection();
            var previousCaCertPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCert);
            importExportCertificate.SaveCertificateToPfxFile($"{intermediateCertName}.pfx", password, intermediateCertificate, previousCaCertPublicKey, chain);
            chain.Add(previousCaCertPublicKey);

            Console.WriteLine($"Exported Certificates");

        }
    }
}
