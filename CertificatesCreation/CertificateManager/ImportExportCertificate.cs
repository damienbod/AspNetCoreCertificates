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
        public void SaveCertificateToPfxFile(string filename, string password,
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
            File.WriteAllBytes(filename, certBytes);
        }

        public X509Certificate2 ExportCertificatePublicKey(X509Certificate2 certificate)
        {
            var publicKeyBytes = certificate.Export(X509ContentType.Cert);
            var signingCertWithoutPrivateKey = new X509Certificate2(publicKeyBytes);
            return signingCertWithoutPrivateKey;
        }

        public (X509Certificate2 certificate, X509Certificate2Collection collection)
            LoadCertificateAndCollectionFromPfx(string pfxFileName, string password)
        {
            if (string.IsNullOrEmpty(pfxFileName))
            {
                throw new ArgumentException($"{nameof(pfxFileName)} must be a valid filename.", nameof(pfxFileName));
            }
            if (!File.Exists(pfxFileName))
            {
                throw new FileNotFoundException($"{pfxFileName} does not exist. Cannot load certificate from non-existing file.", pfxFileName);
            }
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(
                pfxFileName,
                password,
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
                Console.WriteLine($"ERROR: {pfxFileName} did not " +
                    $"contain any certificate with a private key.");
                return (null, null);
            }
            else
            {
                Console.WriteLine($"Using certificate {certificate.Thumbprint} " +
                    $"{certificate.Subject}");
                return (certificate, outcollection);
            }

        }
    }
}
