using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreateAngularVueJsDevelopmentCertificates
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var _createCertificatesRsa = serviceProvider.GetService<CreateCertificatesRsa>();

            // Create development certificate for localhost
            var devCertificate = _createCertificatesRsa
                .CreateDevelopmentCertificate("localhost", 10);

            devCertificate.FriendlyName = "localhost development";

            string password = "1234";
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

            // full pfx with password
            var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, devCertificate);
            File.WriteAllBytes("dev_localhost.pfx", rootCertInPfxBtyes);

            // private key
            var exportRsaPrivateKeyPem = importExportCertificate.PemExportRsaPrivateKey(devCertificate);
            File.WriteAllText($"dev_localhost.key", exportRsaPrivateKeyPem);

            // public key certificate as pem
            var exportPublicKeyCertificatePem = importExportCertificate.PemExportPublicKeyCertificate(devCertificate);
            File.WriteAllText($"dev_localhost.pem", exportPublicKeyCertificatePem);

            // public key certificate as cer
            var exportCertificatePublicKey = importExportCertificate.ExportCertificatePublicKey(devCertificate);
            var exportCertificatePublicKeyBytes = exportCertificatePublicKey.Export(X509ContentType.Cert);
            File.WriteAllBytes($"dev_localhost.cer", exportCertificatePublicKeyBytes);

        }
    }
}
