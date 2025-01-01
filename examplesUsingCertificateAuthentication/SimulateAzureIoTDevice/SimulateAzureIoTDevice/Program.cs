using CertificateManager;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimulateAzureIoTDevice;

class Program
{
    static readonly string _directory = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
    static readonly string _pathToCerts = $"{_directory}\\";

    // Define the device
    private static readonly string iotHubUrl = "damienbod-iothub.azure-devices.net";
    private static readonly TransportType transportType = TransportType.Amqp;

    private const int TEMPERATURE_THRESHOLD = 30;
    private static readonly int MESSAGE_COUNT = 5;
    private static readonly Random rnd = new();
    private static float temperature;
    private static float humidity;

    static void Main(string[] args)
    {
        try
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();
            var iec = serviceProvider.GetService<ImportExportCertificate>();

            #region pem

            var deviceNamePem = "final-measurement";

            var certPem = File.ReadAllText($"{_pathToCerts}{deviceNamePem}-public.pem");
            var eccPem = File.ReadAllText($"{_pathToCerts}{deviceNamePem}-private.pem");
            var cert = X509Certificate2.CreateFromPem(certPem, eccPem);

            // setup deviceCert windows store export 
            var pemDeviceCertPrivate = iec!.PemExportPfxFullCertificate(cert);
            var certDevice = iec.PemImportCertificate(pemDeviceCertPrivate);

            #endregion pem

            #region pfx
            //var passwordPfx = "OdIdcwdw";
            //var deviceNamePfx = "de5"; // "testdevice01";
            //var certTestdevicePfx = X509CertificateLoader.LoadPkcs12FromFile($"{_pathToCerts}{deviceNamePfx}.pfx", passwordPfx);
            #endregion pfx

            var auth = new DeviceAuthenticationWithX509Certificate(deviceNamePem, certDevice);
            var deviceClient = DeviceClient.Create(iotHubUrl, auth, transportType);

            if (deviceClient == null)
            {
                Console.WriteLine("Failed to create DeviceClient!");
            }
            else
            {
                Console.WriteLine("Successfully created DeviceClient!");

                deviceClient.UpdateReportedPropertiesAsync(
                    new TwinCollection("{ \"updatedby\":\"" + "simiDam" + "\", \"timeZone\":\"" + TimeZoneInfo.Local.DisplayName + "\" }")
                );
                SendEvent(deviceClient).Wait();
            }

            Console.WriteLine("Exiting...\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in sample: {0}", ex.Message);
        }
    }

    static async Task SendEvent(DeviceClient deviceClient)
    {
        var deviceName = "simi1";
        string dataBuffer;
        Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);

        for (int count = 0; count < MESSAGE_COUNT; count++)
        {
            temperature = rnd.Next(20, 35);
            humidity = rnd.Next(60, 80);
            dataBuffer = string.Format("{{\"deviceId\":\"{0}\",\"messageId\":{1},\"temperature\":{2},\"humidity\":{3}}}", deviceName, count, temperature, humidity);
            var eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
            eventMessage.Properties.Add("temperatureAlert", (temperature > TEMPERATURE_THRESHOLD) ? "true" : "false");
            Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

            try
            {
                await deviceClient.SendEventAsync(eventMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SendEventAsync: {0}", ex.Message);
            }
        }
    }
}
