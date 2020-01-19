using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateManager
{
    /// <summary>
    /// API used to import and export certificates in bytes for the cer and pfc format
    /// </summary>
    public class ImportExportCertificate
    {
        /// <summary>
        /// Exports the certificate public key which can then be saved as a cer file
        /// </summary>
        /// <param name="certificate">X509Certificate2 cert</param>
        /// <returns></returns>
        public X509Certificate2 ExportCertificatePublicKey(X509Certificate2 certificate)
        {
            var publicKeyBytes = certificate.Export(X509ContentType.Cert);
            var signingCertWithoutPrivateKey = new X509Certificate2(publicKeyBytes);
            return signingCertWithoutPrivateKey;
        }

        /// <summary>
        /// Exports a root certificate
        /// </summary>
        /// <param name="password">password used to create export</param>
        /// <param name="certificate">certificate to export</param>
        /// <returns>pfx or Pkcs12 byte[]</returns>
        public byte[] ExportSelfSignedCertificatePfx(string password, X509Certificate2 certificate)
        {
            return CertificateToPfx(password, certificate, null, null);
        }

        /// <summary>
        /// Exports a root certificate
        /// </summary>
        /// <param name="password">password used to create export</param>
        /// <param name="certificate">certificate to export</param>
        /// <returns>pfx or Pkcs12 byte[]</returns>
        public byte[] ExportRootPfx(string password, X509Certificate2 certificate)
        {
            return CertificateToPfx(password, certificate, null, null);
        }

        /// <summary>
        /// Exports a chained certificate in byte[] pfx , Pkcs12 format
        /// </summary>
        /// <param name="password">password used to create the export</param>
        /// <param name="certificate">certificate to export</param>
        /// <param name="signingCert">cert with the parent chain</param>
        /// <returns>pfx or Pkcs12 byte[]</returns>
        public byte[] ExportChainedCertificatePfx(string password, X509Certificate2 certificate, X509Certificate2 signingCert)
        {
            var caCertCollection = GetCertificateCollection(signingCert, password);
            var publicKeySigningCert = ExportCertificatePublicKey(signingCert);
            return CertificateToPfx(password, certificate, publicKeySigningCert, caCertCollection);
        }

        /// <summary>
        /// Exports a certificate as a base64 string in the pem format string
        /// </summary>
        /// <param name="cert">certificate to export</param>
        /// <returns>A pem certificate as a string</returns>
        public string ExportToPem(X509Certificate cert, string password = null)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            if(string.IsNullOrEmpty(password))
            {
                builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), 
                    Base64FormattingOptions.InsertLineBreaks));
            }
            else
            {
                builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert, password), 
                    Base64FormattingOptions.InsertLineBreaks));
            }
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        public X509Certificate ImportPemCertificate(string pemCertificate, string password = null)
        {
            var sb = new StringBuilder(pemCertificate);
          // to do remove stuff
            var certBytes = Convert.FromBase64String(pemCertificate);

            if (string.IsNullOrEmpty(password))
            {
                var certificate = new X509Certificate2(certBytes);
                return certificate;
            }
            else
            {
                var certificate = new X509Certificate2(certBytes, password);
                return certificate;
            }
        }

        private byte[] CertificateToPfx(string password,
            X509Certificate2 certificate, 
            X509Certificate2 signingCertificate,
            X509Certificate2Collection chain)
        {
            var certCollection = new X509Certificate2Collection(certificate);
            if (chain != null)
            {
                certCollection.AddRange(chain);
            }

            if (signingCertificate != null)
            {
                var signingCertWithoutPrivateKey = ExportCertificatePublicKey(signingCertificate);
                certCollection.Add(signingCertWithoutPrivateKey);
            }

            return certCollection.Export(X509ContentType.Pkcs12, password);
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
