using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CertificateManager;
using System.Security.Cryptography;

namespace CreateChainedCertsConsoleDemo
{
    public static class LowLevelApiExamples
    {
        public static void Run()
        {
            Console.WriteLine("Create Root Certificate");

            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            //new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            //new Oid("1.3.6.1.5.5.7.3.2")  // TLS Client auth
            //new Oid("1.3.6.1.5.5.7.3.3")  // Code signing 
            //new Oid("1.3.6.1.5.5.7.3.4")  // Email
            //new Oid("1.3.6.1.5.5.7.3.8")  // Timestamping  

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

            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, rootCert);
            File.WriteAllBytes("localhost_root_l1.pfx", rootCertInPfxBtyes);

            var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCert);
            var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"localhost_root_l1.cer", rootPublicKeyBytes);

            var intermediateCertInPfxBtyes = importExportCertificate.ExportCertificatePfx(password, intermediateCertificate, rootCert);
            File.WriteAllBytes("localhost_intermediate_l2.pfx", intermediateCertInPfxBtyes);

            var intermediateCertL3InPfxBtyes = importExportCertificate.ExportCertificatePfx(password, intermediateCertificateLevel3, intermediateCertificate);
            File.WriteAllBytes("localhost_intermediate_l3.pfx", intermediateCertL3InPfxBtyes);

            var deviceCertL4InPfxBtyes = importExportCertificate.ExportCertificatePfx(password, deviceCertificate, intermediateCertificateLevel3);
            File.WriteAllBytes("devicel4.pfx", deviceCertL4InPfxBtyes);

            var deviceVerificationCert = deviceCertCreator.CreateDeviceCertificate(
               DeviceCertConfig.DistinguishedName,
               DeviceCertConfig.BasicConstraints,
               DeviceCertConfig.ValidityPeriod,
               DeviceCertConfig.SubjectAlternativeName,
               rootCert,
               enhancedKeyUsages);

            deviceVerificationCert.FriendlyName = "device verification cert l4";

            var publicKeyBytes = deviceVerificationCert.Export(X509ContentType.Cert);
            File.WriteAllBytes("deviceVerificationCert.cer", publicKeyBytes);

            Console.WriteLine($"Exported Certificates");

        }
    }
}
