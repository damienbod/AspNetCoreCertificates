using System.Security.Cryptography;

namespace CertificateManager.Models
{
    public class ECDsaConfiguration
    {
        /// <summary>
        /// RSA 1024, 2048 or 4096
        /// </summary>
        public int KeySize { get; set; } = 256;

        public HashAlgorithmName HashAlgorithmName { get; set; } = HashAlgorithmName.SHA256;
        
    }
}
