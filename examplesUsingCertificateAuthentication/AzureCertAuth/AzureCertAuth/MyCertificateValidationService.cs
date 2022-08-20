using System.Security.Cryptography.X509Certificates;

namespace AzureCertAuth;

public class MyCertificateValidationService 
{
    private readonly ILogger<MyCertificateValidationService> _logger;

    public MyCertificateValidationService(ILogger<MyCertificateValidationService> logger)
    {
        _logger = logger;
    }

    public bool ValidateCertificate(X509Certificate2 clientCertificate)
    {
        return CheckIfThumbprintIsValid(clientCertificate);
    }

    private bool CheckIfThumbprintIsValid(X509Certificate2 clientCertificate)
    {
        var listOfValidThumbprints = new List<string>
        {
            "723A4D916F008B8464E1D314C6FABC1CB1E926BD"
        };

        if (listOfValidThumbprints.Contains(clientCertificate.Thumbprint))
        {
            _logger.LogInformation($"Custom auth-success for certificate  {clientCertificate.FriendlyName} {clientCertificate.Thumbprint}");

            return true;
        }

        _logger.LogWarning($"auth failed for certificate  {clientCertificate.FriendlyName} {clientCertificate.Thumbprint}");

        return false;
    }
}
