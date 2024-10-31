using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace IoTHubCreateDeviceCertificate;

/// <summary>
/// https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-x509-get-started
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddCertificateManager()
            .BuildServiceProvider();

        var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

        var intermediate = new X509Certificate2("intermediate.pfx", "1234");

        // use lowercase for dps
        var testDevice01 = createClientServerAuthCerts.NewDeviceChainedCertificate(
            new DistinguishedName { CommonName = "testdevice01" },
            new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
            "testdevice01", intermediate);
        testDevice01.FriendlyName = "IoT device testDevice01";

        string password = "1234";
        var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

        var deviceInPfxBytes = importExportCertificate.ExportChainedCertificatePfx(password, testDevice01, intermediate);
        File.WriteAllBytes("testDevice01.pfx", deviceInPfxBytes);

        Console.WriteLine("Certificates exported to pfx file");
    }
}
