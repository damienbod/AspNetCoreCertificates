using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreateChainedCertsConsoleDemo;

public static class LowLevelApiExamples
{
    public static void Run()
    {
        Console.WriteLine("Create Root Certificate");

        var serviceProvider = new ServiceCollection()
            .AddCertificateManager()
            .BuildServiceProvider();

        // OidLookup.ClientAuthentication
        // OidLookup.ServerAuthentication 
        // OidLookup.CodeSigning,
        // OidLookup.SecureEmail,
        // OidLookup.TimeStamping 
        var enhancedKeyUsages = new OidCollection {
            OidLookup.ClientAuthentication,
            OidLookup.ServerAuthentication
        };

        var createCertificates = serviceProvider.GetService<CreateCertificates>();

        // Create the root self signed cert
        var rootCert = createCertificates.NewECDsaSelfSignedCertificate(
            RootCertConfig.DistinguishedName,
            RootCertConfig.BasicConstraints,
            RootCertConfig.ValidityPeriod,
            RootCertConfig.SubjectAlternativeName,
            enhancedKeyUsages,
            RootCertConfig.X509KeyUsageFlags,
            new ECDsaConfiguration());

        rootCert.FriendlyName = "localhost root l1";

        // Create an intermediate chained cert
        var intermediateCertificate = createCertificates.NewECDsaChainedCertificate(
            IntermediateCertConfig.DistinguishedName,
            IntermediateCertConfig.BasicConstraints,
            IntermediateCertConfig.ValidityPeriod,
            IntermediateCertConfig.SubjectAlternativeName,
            rootCert,
            enhancedKeyUsages,
            IntermediateCertConfig.X509KeyUsageFlags,
            new ECDsaConfiguration());

        intermediateCertificate.FriendlyName = "intermediate from root l2";

        // Create a second intermediate chained cert
        var intermediateCertificateLevel3 = createCertificates.NewECDsaChainedCertificate(
            IntermediateLevel3CertConfig.DistinguishedName,
            IntermediateLevel3CertConfig.BasicConstraints,
            IntermediateLevel3CertConfig.ValidityPeriod,
            IntermediateLevel3CertConfig.SubjectAlternativeName,
            intermediateCertificate,
            enhancedKeyUsages,
            IntermediateLevel3CertConfig.X509KeyUsageFlags,
            new ECDsaConfiguration());

        intermediateCertificateLevel3.FriendlyName = "intermediate l3 from intermediate";

        // Create a device chained cert
        var deviceCertificate = createCertificates.NewECDsaChainedCertificate(
            DeviceCertConfig.DistinguishedName,
            DeviceCertConfig.BasicConstraints,
            DeviceCertConfig.ValidityPeriod,
            DeviceCertConfig.SubjectAlternativeName,
            intermediateCertificateLevel3,
            enhancedKeyUsages,
            DeviceCertConfig.X509KeyUsageFlags,
            new ECDsaConfiguration());

        deviceCertificate.FriendlyName = "device cert l4";

        string password = "1234";
        var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

        var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, rootCert);
        File.WriteAllBytes("localhost_root_l1.pfx", rootCertInPfxBtyes);

        var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCert);
        var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
        File.WriteAllBytes($"localhost_root_l1.cer", rootPublicKeyBytes);

        var intermediateCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, intermediateCertificate, rootCert);
        File.WriteAllBytes("localhost_intermediate_l2.pfx", intermediateCertInPfxBtyes);

        var intermediateCertL3InPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, intermediateCertificateLevel3, intermediateCertificate);
        File.WriteAllBytes("localhost_intermediate_l3.pfx", intermediateCertL3InPfxBtyes);

        var deviceCertL4InPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, deviceCertificate, intermediateCertificateLevel3);
        File.WriteAllBytes("devicel4.pfx", deviceCertL4InPfxBtyes);

        // Create a device validation cert
        var deviceVerificationCert = createCertificates.NewECDsaChainedCertificate(
           DeviceCertConfig.DistinguishedName,
           DeviceCertConfig.BasicConstraints,
           DeviceCertConfig.ValidityPeriod,
           DeviceCertConfig.SubjectAlternativeName,
           rootCert,
           enhancedKeyUsages,
           DeviceCertConfig.X509KeyUsageFlags,
           new ECDsaConfiguration());

        deviceVerificationCert.FriendlyName = "device verification cert l4";

        var publicKeyBytes = deviceVerificationCert.Export(X509ContentType.Cert);
        File.WriteAllBytes("deviceVerificationCert.cer", publicKeyBytes);

        Console.WriteLine($"Exported Certificates");

    }
}
