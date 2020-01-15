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

            rootCert.FriendlyName = "localhost root l1";

            Console.WriteLine($"Created Root Certificate {rootCert.SubjectName}");


            var icCreator = serviceProvider.GetService<IntermediateCertificate>();

            var intermediateCertificate = icCreator.CreateIntermediateCertificate(
                IntermediateCertConfig.DistinguishedName,
                IntermediateCertConfig.BasicConstraints,
                IntermediateCertConfig.ValidityPeriod,
                IntermediateCertConfig.SubjectAlternativeName,
                rootCert);

            intermediateCertificate.FriendlyName = "intermediate from root l2";

            Console.WriteLine($"Created Intermediate Certificate {intermediateCertificate.SubjectName}");

            var intermediateCertificateLevel3 = icCreator.CreateIntermediateCertificate(
                IntermediateLevel3CertConfig.DistinguishedName,
                IntermediateLevel3CertConfig.BasicConstraints,
                IntermediateLevel3CertConfig.ValidityPeriod,
                IntermediateLevel3CertConfig.SubjectAlternativeName,
                intermediateCertificate);

            intermediateCertificateLevel3.FriendlyName = "intermediate l3 from intermediate";

            var deviceCertCreator = serviceProvider.GetService<DeviceCertificate>();

            var deviceCertificate = deviceCertCreator.CreateDeviceCertificate(
                DeviceCertConfig.DistinguishedName,
                DeviceCertConfig.BasicConstraints,
                DeviceCertConfig.ValidityPeriod,
                DeviceCertConfig.SubjectAlternativeName,
                intermediateCertificateLevel3);

            deviceCertificate.FriendlyName = "device cert l4";
            

            string password = "1234";
      
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            importExportCertificate.SaveCertificateToPfxFile(
                "localhost_root_l1.pfx", password, rootCert, null, null);
            var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCert);
            var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"localhost_root_l1.cer", rootPublicKeyBytes);

            var chain = new X509Certificate2Collection();
            var previousCaCertPublicKeyRoot =  importExportCertificate.ExportCertificatePublicKey(rootCert);
            importExportCertificate.SaveCertificateToPfxFile(
                "localhost_intermediate_l2.pfx", password, intermediateCertificate, previousCaCertPublicKeyRoot, chain);
            
            chain.Add(previousCaCertPublicKeyRoot);
            var previousCaCertPublicKeyIntermediate = importExportCertificate.ExportCertificatePublicKey(intermediateCertificate);
            importExportCertificate.SaveCertificateToPfxFile("localhost_intermediate_l3.pfx", 
                password, intermediateCertificateLevel3, previousCaCertPublicKeyIntermediate, chain);

            //var (certificate, collection) = 
            //   importExportCertificate.LoadCertificateAndCollectionFromPfx(SigningCertPfxFile, password);

            importExportCertificate.SaveCertificateToPfxFile(
                $"devicel4.pfx", password, deviceCertificate, intermediateCertificateLevel3, chain);

            Console.WriteLine($"Exported Certificates");

        }
    }
}
