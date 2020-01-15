using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CertificateManager;
using System.Security.Cryptography;

namespace CertificatesCreation
{
    public static class LowLevelApis
    {
        public static void Run()
        {
            Console.WriteLine("Create Root Certificate");

            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            };

            var rcCreator = serviceProvider.GetService<RootCertificate>();

            var rootCert = rcCreator.CreateRootCertificate(
                RootCertConfig.DistinguishedName,
                RootCertConfig.BasicConstraints,
                RootCertConfig.ValidityPeriod,
                RootCertConfig.SubjectAlternativeName,
                enhancedKeyUsages);

            rootCert.FriendlyName = "localhost root l1";

            var icCreator = serviceProvider.GetService<IntermediateCertificate>();

            var intermediateCertificate = icCreator.CreateIntermediateCertificate(
                IntermediateCertConfig.DistinguishedName,
                IntermediateCertConfig.BasicConstraints,
                IntermediateCertConfig.ValidityPeriod,
                IntermediateCertConfig.SubjectAlternativeName,
                rootCert,
                enhancedKeyUsages);

            intermediateCertificate.FriendlyName = "intermediate from root l2";

            var intermediateCertificateLevel3 = icCreator.CreateIntermediateCertificate(
                IntermediateLevel3CertConfig.DistinguishedName,
                IntermediateLevel3CertConfig.BasicConstraints,
                IntermediateLevel3CertConfig.ValidityPeriod,
                IntermediateLevel3CertConfig.SubjectAlternativeName,
                intermediateCertificate,
                enhancedKeyUsages);

            intermediateCertificateLevel3.FriendlyName = "intermediate l3 from intermediate";

            var deviceCertCreator = serviceProvider.GetService<DeviceCertificate>();

            var deviceCertificate = deviceCertCreator.CreateDeviceCertificate(
                DeviceCertConfig.DistinguishedName,
                DeviceCertConfig.BasicConstraints,
                DeviceCertConfig.ValidityPeriod,
                DeviceCertConfig.SubjectAlternativeName,
                intermediateCertificateLevel3,
                enhancedKeyUsages);

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

            importExportCertificate.SaveCertificateToPfxFile(
                $"devicel4.pfx", password, deviceCertificate, intermediateCertificateLevel3, chain);

            Console.WriteLine($"Exported Certificates");

        }
    }
}
