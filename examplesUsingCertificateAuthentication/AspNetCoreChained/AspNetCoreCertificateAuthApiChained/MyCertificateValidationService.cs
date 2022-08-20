using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuthApi;

public class MyCertificateValidationService 
{
    public bool ValidateCertificate(X509Certificate2 clientCertificate)
    {
        return CheckIfThumbprintIsValid(clientCertificate);
    }

    private bool CheckIfThumbprintIsValid(X509Certificate2 clientCertificate)
    {
        var listOfValidThumbprints = new List<string>
        {
            "2F002F39CCC224DF70FE4EE54195B2E6FE6FB5D2" 
        };

        if (listOfValidThumbprints.Contains(clientCertificate.Thumbprint))
        {
            return true;
        }

        return false;
    }
}
