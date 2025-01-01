using Microsoft.Azure.Devices.Client;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimulateAzureIoTDevice;

class Program
{
    private static readonly int MESSAGE_COUNT = 5;
    private const int TEMPERATURE_THRESHOLD = 30;
    private static readonly string deviceId = "TestDevice01";
    private static float temperature;
    private static float humidity;
    private static readonly Random rnd = new();

    static void Main(string[] args)
    {
        try
        {
            var cert = X509CertificateLoader.LoadPkcs12FromFile(@"testDevice01.pfx", "1234");
            var auth = new DeviceAuthenticationWithX509Certificate("TestDevice01", cert);
            var deviceClient = DeviceClient.Create("damienbod.azure-devices.net", auth, TransportType.Amqp_Tcp_Only);

            if (deviceClient == null)
            {
                Console.WriteLine("Failed to create DeviceClient!");
            }
            else
            {
                Console.WriteLine("Successfully created DeviceClient!");
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
        string dataBuffer;
        Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);

        for (int count = 0; count < MESSAGE_COUNT; count++)
        {
            temperature = rnd.Next(20, 35);
            humidity = rnd.Next(60, 80);
            dataBuffer = string.Format("{{\"deviceId\":\"{0}\",\"messageId\":{1},\"temperature\":{2},\"humidity\":{3}}}", deviceId, count, temperature, humidity);
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
            eventMessage.Properties.Add("temperatureAlert", (temperature > TEMPERATURE_THRESHOLD) ? "true" : "false");
            Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

            await deviceClient.SendEventAsync(eventMessage);
        }
    }
}
