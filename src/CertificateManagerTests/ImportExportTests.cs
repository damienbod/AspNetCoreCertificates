using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace CertificateManagerTests
{
    public class ImportExportTests
    {
        private (X509Certificate2 root, X509Certificate2 intermediate, X509Certificate2 server, X509Certificate2 client) SetupCerts()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var rootCaL1 = createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName { CommonName = "root dev", Country = "IT" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            rootCaL1.FriendlyName = "developement root L1 certificate";

            // Intermediate L2 chained from root L1
            var intermediateCaL2 = createClientServerAuthCerts.NewIntermediateChainedCertificate(
                new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                2, "localhost", rootCaL1);
            intermediateCaL2.FriendlyName = "developement Intermediate L2 certificate";

            // Server, Client L3 chained from Intermediate L2
            var serverL3 = createClientServerAuthCerts.NewServerChainedCertificate(
                new DistinguishedName { CommonName = "server", Country = "DE" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);

            var clientL3 = createClientServerAuthCerts.NewClientChainedCertificate(
                new DistinguishedName { CommonName = "client", Country = "IE" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);
            serverL3.FriendlyName = "developement server L3 certificate";
            clientL3.FriendlyName = "developement client L3 certificate";

            return (rootCaL1, intermediateCaL2, serverL3, clientL3);
        }

        [Fact]
        public void ImportExportCrtSelfSignedPem()
        {
            var (root, intermediate, server, client) = SetupCerts();
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();
            var importExport = serviceProvider.GetService<ImportExportCertificate>();

            var crtPem = importExport.PemExportPfxFullCertificate(root);
            var roundTripCertificate = importExport.PemImportCertificate(crtPem);

            Assert.Equal(root.Subject, roundTripCertificate.Subject);
            Assert.True(roundTripCertificate.HasPrivateKey);

        }

        [Fact]
        public void ImportExportPasswordCrtPem()
        {
            var (root, intermediate, server, client) = SetupCerts();
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();
            var importExport = serviceProvider.GetService<ImportExportCertificate>();

            var crtPem = importExport.PemExportPfxFullCertificate(intermediate, "23456");
            var roundTripCertificate = importExport.PemImportCertificate(crtPem, "23456");

            Assert.Equal(intermediate.Subject, roundTripCertificate.Subject);
            Assert.True(intermediate.HasPrivateKey);
            Assert.True(roundTripCertificate.HasPrivateKey);
        }

        [Fact]
        public void ExportEDAsaPublicKeyCertificatePem()
        {
            var (root, intermediate, server, client) = SetupCerts();
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();
            var importExport = serviceProvider.GetService<ImportExportCertificate>();

            var crtPem = importExport.PemExportPublicKeyCertificate(intermediate);
            var roundTripCertificate = importExport.PemImportCertificate(crtPem);

            Assert.Equal(intermediate.Subject, roundTripCertificate.Subject);
            Assert.True(intermediate.HasPrivateKey);
            Assert.False(roundTripCertificate.HasPrivateKey);
        }

        [Fact]
        public void ImportExportIncorrectPasswordCrtPem()
        {
            var (root, intermediate, server, client) = SetupCerts();
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();
            var importExport = serviceProvider.GetService<ImportExportCertificate>();

            var exception = Assert.Throws<ArgumentException> (() =>
            {
                try
                {
                    var crtPem = importExport.PemExportPfxFullCertificate(intermediate, "23HHHH456");
                    var roundTripCertificate = importExport.PemImportCertificate(crtPem, "23456");
                }
                catch(Exception ex)
                {
                    // internal Internal.Cryptography.CryptoThrowHelper+WindowsCryptographicException : The specified network password is not correct.
                    Assert.Equal("The specified network password is not correct.", ex.Message);
                    throw new ArgumentException();
                }
            });

        }

        [Fact]
        public void ImportExportExportFullPfxPem()
        {
            var (root, intermediate, server, client) = SetupCerts();
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();
            var importExport = serviceProvider.GetService<ImportExportCertificate>();

            var pfxPem = importExport.PemExportPfxFullCertificate(intermediate);

            var roundTripPfxPem = importExport.PemImportCertificate(pfxPem);

            Assert.Equal(intermediate.Subject, roundTripPfxPem.Subject);
            Assert.True(intermediate.HasPrivateKey);
            Assert.True(roundTripPfxPem.HasPrivateKey);
        }

        [Fact]
        public void ImportExportRsaCertPublicKeyPem()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var ccRsa = serviceProvider.GetService<CreateCertificatesRsa>();
            var importExport = serviceProvider.GetService<ImportExportCertificate>();

            var rsaCert = ccRsa.CreateDevelopmentCertificate("localhost", 2, 2048);

            var publicKeyPem = importExport.PemExportPublicKeyCertificate(rsaCert);
            var roundTripPublicKeyPem = importExport.PemImportCertificate(publicKeyPem);

            Assert.Equal(rsaCert.Subject, roundTripPublicKeyPem.Subject);
            Assert.True(rsaCert.HasPrivateKey);
            Assert.False(roundTripPublicKeyPem.HasPrivateKey);
        }
    }
}
