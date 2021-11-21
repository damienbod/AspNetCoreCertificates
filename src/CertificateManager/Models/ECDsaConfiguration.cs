using System.Security.Cryptography;

namespace CertificateManager.Models
{
    public class ECDsaConfiguration
    {
        /// <summary>
        /// EC 256, 384
        /// </summary>
        public int KeySize { get; set; } = 256;

        public HashAlgorithmName HashAlgorithmName { get; set; } = HashAlgorithmName.SHA256;

    }
}
