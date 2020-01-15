using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    /// <summary>
    /// https://github.com/rwatjen/AzureIoTDPSCertificates
    /// </summary>
    public class ImportExportCertificate
    {
        public X509Certificate2 ExportCertificatePublicKey(X509Certificate2 certificate)
        {
            var publicKeyBytes = certificate.Export(X509ContentType.Cert);
            var signingCertWithoutPrivateKey = new X509Certificate2(publicKeyBytes);
            return signingCertWithoutPrivateKey;
        }

        public byte[] ExportRootPfx(string password, X509Certificate2 certificate)
        {
            return CertificateToPfx(password, certificate, null, null);
        }

        public byte[] ExportCertificatePfx(string password, X509Certificate2 certificate, X509Certificate2 signingCert)
        {
            var caCertCollection = GetCertificateCollection(signingCert, password);
            var publicKeySigningCert = ExportCertificatePublicKey(signingCert);
            return CertificateToPfx(password, certificate, publicKeySigningCert, caCertCollection);
        }

        private byte[] CertificateToPfx(string password,
            X509Certificate2 certificate, X509Certificate2 signingCert,
            X509Certificate2Collection chain)
        {
            var certCollection = new X509Certificate2Collection(certificate);
            if (chain != null)
            {
                certCollection.AddRange(chain);
            }
            if (signingCert != null)
            {
                var signingCertWithoutPrivateKey = ExportCertificatePublicKey(signingCert);
                certCollection.Add(signingCertWithoutPrivateKey);

            }
            var certBytes = certCollection.Export(X509ContentType.Pkcs12, password);
            return certBytes;
        }

        private X509Certificate2Collection GetCertificateCollection(X509Certificate2 inputCert, string password)
        {
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(inputCert.GetRawCertData(), password,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);

            X509Certificate2 certificate = null;
            var outcollection = new X509Certificate2Collection();
            foreach (X509Certificate2 element in certificateCollection)
            {
                Console.WriteLine($"Found certificate: {element?.Thumbprint} " +
                    $"{element?.Subject}; PrivateKey: {element?.HasPrivateKey}");
                if (certificate == null && element.HasPrivateKey)
                {
                    certificate = element;
                }
                else
                {
                    outcollection.Add(element);
                }
            }

            if (certificate == null)
            {
                return null;
            }
            else
            {
                return outcollection;
            }
        }
    }
}
