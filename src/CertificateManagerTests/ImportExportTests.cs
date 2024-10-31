using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace CertificateManagerTests;

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

        var exception = Assert.Throws<ArgumentException>(() =>
       {
           try
           {
               var crtPem = importExport.PemExportPfxFullCertificate(intermediate, "23HHHH456");
               var roundTripCertificate = importExport.PemImportCertificate(crtPem, "23456");
           }
           catch (Exception ex)
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

    [Fact]
    public void ImportExportRsaPrivateKeyPublicKeyPairPem()
    {
        var serviceProvider = new ServiceCollection()
            .AddCertificateManager()
            .BuildServiceProvider();

        var ccRsa = serviceProvider.GetService<CreateCertificatesRsa>();
        var importExport = serviceProvider.GetService<ImportExportCertificate>();

        var rsaCert = ccRsa.CreateDevelopmentCertificate("localhost", 2, 2048);

        var publicKeyPem = importExport.PemExportPublicKeyCertificate(rsaCert);
        var rsaPrivateKeyPem = importExport.PemExportRsaPrivateKey(rsaCert);

        var roundTripPublicKeyPem = importExport.PemImportCertificate(publicKeyPem);
        var roundTripRsaPrivateKeyPem = importExport.PemImportPrivateKey(rsaPrivateKeyPem);

        var roundTripFullCert =
            importExport.CreateCertificateWithPrivateKey(roundTripPublicKeyPem, roundTripRsaPrivateKeyPem, "1234");

        Assert.Equal(rsaCert.Subject, roundTripPublicKeyPem.Subject);
        Assert.Equal(rsaCert.Thumbprint, roundTripFullCert.Thumbprint);
        Assert.True(roundTripFullCert.HasPrivateKey);
        Assert.Equal("sha256RSA", roundTripFullCert.SignatureAlgorithm.FriendlyName);
    }

    [Fact]
    public void ImportExportECPrivateKeyPublicKeyPairPem()
    {
        var (root, intermediate, server, client) = SetupCerts();

        var serviceProvider = new ServiceCollection()
            .AddCertificateManager()
            .BuildServiceProvider();

        var importExport = serviceProvider.GetService<ImportExportCertificate>();

        var publicKeyPem = importExport.PemExportPublicKeyCertificate(root);
        var ecPrivateKeyPem = importExport.PemExportECPrivateKey(root);

        var roundTripPublicKeyPem = importExport.PemImportCertificate(publicKeyPem);
        var roundTripRsaPrivateKeyPem = importExport.PemImportPrivateKey(ecPrivateKeyPem);

        var roundTripFullCert =
            importExport.CreateCertificateWithPrivateKey(
                roundTripPublicKeyPem,
                roundTripRsaPrivateKeyPem, "1234");

        Assert.Equal(root.Subject, roundTripPublicKeyPem.Subject);
        Assert.Equal(root.Thumbprint, roundTripFullCert.Thumbprint);
        Assert.True(roundTripFullCert.HasPrivateKey);
        Assert.Equal("sha256ECDSA", roundTripFullCert.SignatureAlgorithm.FriendlyName);
    }

    [Fact]
    public void ImportExportSingleChainedECPrivateKeyPublicKeyPairPem()
    {
        var (root, intermediate, server, client) = SetupCerts();

        var serviceProvider = new ServiceCollection()
            .AddCertificateManager()
            .BuildServiceProvider();

        var importExport = serviceProvider.GetService<ImportExportCertificate>();

        var publicKeyPem = importExport.PemExportPublicKeyCertificate(server);
        var ecPrivateKeyPem = importExport.PemExportECPrivateKey(server);

        var roundTripPublicKeyPem = importExport.PemImportCertificate(publicKeyPem);
        var roundTripRsaPrivateKeyPem = importExport.PemImportPrivateKey(ecPrivateKeyPem);

        var roundTripFullCert =
            importExport.CreateCertificateWithPrivateKey(
                roundTripPublicKeyPem,
                roundTripRsaPrivateKeyPem);

        Assert.Equal(server.Subject, roundTripPublicKeyPem.Subject);
        Assert.Equal(server.Thumbprint, roundTripFullCert.Thumbprint);
        Assert.True(roundTripFullCert.HasPrivateKey);
        Assert.Equal("sha256ECDSA", roundTripFullCert.SignatureAlgorithm.FriendlyName);
    }

    [Fact]
    public void ImportDerPem()
    {

        var serviceProvider = new ServiceCollection()
            .AddCertificateManager()
            .BuildServiceProvider();

        var importExport = serviceProvider.GetService<ImportExportCertificate>();

        var certstring = @"-----BEGIN CERTIFICATE-----
MIIEBzCCAu+gAwIBAgIQLlpk6CS8R0Z09GPVciC2qjANBgkqhkiG9w0BAQsFADAyMTAwLgYDVQQD
EydEaWVnbyBJbnN0YW5jZSBJZGVudGl0eSBJbnRlcm1lZGlhdGUgQ0EwHhcNMjAwMjE5MTkzMjQy
WhcNMjAwMjIwMTkzMjQyWjCByDGBnjA4BgNVBAsTMW9yZ2FuaXphdGlvbjpkN2FmZTVjYi0yZDQy
LTQ4N2ItYTQxNS1mNDdjMDY2NWYxYmEwMQYDVQQLEypzcGFjZTpmMDNmMmFiMC1jZjMzLTQxNmIt
OTk5Yy1mYjAxYzEyNDc3NTMwLwYDVQQLEyhhcHA6MTA4ZmU0ZDUtYTgzZS00OGI5LWE1NDktMDU0
MmU3MWE0N2E2MSUwIwYDVQQDExw4OTE4MmE5Yi0wNjhmLTQ4NjgtNGYwZS01OWFhMIIBIjANBgkq
hkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAx+3DZQ6LvxB3wM7LWdfRQvj5X75aYMRip/bGlvJVBFvN
cKQqSjijtqHO0d71cLJbZPrtNrmBDLSKGjpzuEq3+EbA2rbL7Tl/WWxdV9dRq2nZQuNF4sh47o6o
ME4CA6S17E7dPBvXPXaRNIhvrGocoga6yhpYBT3iMXKSZnum5KI2qVLXNmCgQVN0G62QSzd3cjsv
+iEa/y3CntnZJ42elq+VfCECin0o35vOzA42GE5zqZokd/FEHg1Cl9flZknIHTxWgW5qQqYzPwZo
w+a26B/kQNJOR4yx1mgQgdmq+6KHdGKQqDhQ9I1I7hTWUXTLx4X0a5pwmWvM2aQKgC/dFwIDAQAB
o4GBMH8wDgYDVR0PAQH/BAQDAgOoMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcDATAfBgNV
HSMEGDAWgBTv0ki5J6EjEz9YdOBuRe+m74pvXDAtBgNVHREEJjAkghw4OTE4MmE5Yi0wNjhmLTQ4
NjgtNGYwZS01OWFhhwQK/3mKMA0GCSqGSIb3DQEBCwUAA4IBAQB0Ao4jbowTGvF1jf5i+9oQBrsA
YKepkZkirIzhKIKVfZEEgMRzm4CVNhB0MG5NiGn7x8XZlCIAO7jVSGOlT51nFx5Q16iyoBHv1r9b
RJ8edDcaHcZ67VuCGppsAyPNobrsMCvNaW4OZsHqQ+H2Tq3btKYqxYw1foWHEJVTJ2ys0ltZ3/IM
phubW4vcUC2Cyn0CMQxZjJs0nMBFy2zgtGAQX7zE6+nKVkzviWjDprHWv90xax08SmPg/OHqYS1s
aBg2iHnjlg9taunpE2KGgPrbU0exPnaV+xYDqRxvoN0cC+mkGuehuVBy/DRtJ3WfevH6sdgqr4BP
cQxzGDwe9ata
-----END CERTIFICATE-----";


        var pemCertImported = importExport.PemImportCertificate(certstring);

        Assert.Equal("sha256RSA", pemCertImported.SignatureAlgorithm.FriendlyName);
    }
}
