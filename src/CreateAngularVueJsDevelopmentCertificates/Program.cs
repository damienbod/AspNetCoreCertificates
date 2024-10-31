using CertificateManager;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace CreateAngularVueJsDevelopmentCertificates;

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
    }
}
