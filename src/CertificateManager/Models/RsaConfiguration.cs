using System.Security.Cryptography;

namespace CertificateManager.Models
{
    public class RsaConfiguration
    {
        /// <summary>
        /// RSA 1024, 2048 or 4096
        /// </summary>
        public int KeySize { get; set; } = 1024;

        /// <summary>
        /// RSASignaturePadding.Pkcs1, RSASignaturePadding.Pss
        /// </summary>
        public RSASignaturePadding RSASignaturePadding { get; set; } = RSASignaturePadding.Pkcs1;

        public HashAlgorithmName HashAlgorithmName { get; set; } = HashAlgorithmName.SHA256;
        
    }
}
