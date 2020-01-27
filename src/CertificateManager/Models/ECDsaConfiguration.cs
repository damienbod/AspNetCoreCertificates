using System.Security.Cryptography;

namespace CertificateManager.Models
{
    public class ECDsaConfiguration
    {
        public int KeySize { get; set; } = 256;

        public HashAlgorithmName HashAlgorithmName { get; set; } = HashAlgorithmName.SHA256;
        
    }
}
