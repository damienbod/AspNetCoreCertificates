using System;
using System.Security.Cryptography;
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
        public string PemExportPfxFullCertificate(X509Certificate2 cert, string password = null)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(PemDecoder.GetBegin(PemTypes.CERTIFICATE));
            if (string.IsNullOrEmpty(password))
            {
                builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Pfx),
                    Base64FormattingOptions.InsertLineBreaks));
            }
            else
            {
                builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Pfx, password),
                    Base64FormattingOptions.InsertLineBreaks));
            }
            builder.AppendLine(PemDecoder.GetEnd(PemTypes.CERTIFICATE));
            return builder.ToString();
        }

        /// <summary>
        /// Export a RSA private key as a pem
        /// PKCS#1
        /// </summary>
        /// <param name="rsaCertificate">certificate which contains the private key</param>
        /// <returns>a pem rsa private key export</returns>
        public string PemExportRsaPrivateKey(X509Certificate2 rsaCertificate)
        {
            var rsa = rsaCertificate.GetRSAPrivateKey();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(PemDecoder.GetBegin(PemTypes.RSA_PRIVATE_KEY));
            builder.AppendLine(Convert.ToBase64String(rsa.ExportRSAPrivateKey(),
                    Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine(PemDecoder.GetEnd(PemTypes.RSA_PRIVATE_KEY));
            return builder.ToString();
        }

        /// <summary>
        /// Export the pem ec private key from a ECDsa certificate
        /// </summary>
        /// <param name="eCDsaCertificate">ECDsa certificate</param>
        /// <returns>pem EC PRIVATE KEY</returns>
        public string PemExportECPrivateKey(X509Certificate2 eCDsaCertificate)
        {
            var eCDsa = eCDsaCertificate.GetECDsaPrivateKey();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(PemDecoder.GetBegin(PemTypes.EC_PRIVATE_KEY));
            builder.AppendLine(Convert.ToBase64String(eCDsa.ExportECPrivateKey(),
                    Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine(PemDecoder.GetEnd(PemTypes.EC_PRIVATE_KEY));
            return builder.ToString();
        }

        /// <summary>
        /// You must use a RSA based certificate for this export to work
        /// PKCS#1
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        //public string PemExportRsaPublicKey(X509Certificate2 cert)
        //{
        //    var rsa = cert.GetRSAPublicKey();

        //    StringBuilder builder = new StringBuilder();
        //    builder.AppendLine(PemDecoder.GetBegin(PemTypes.RSA_PUBLIC_KEY));
        //    builder.AppendLine(Convert.ToBase64String(rsa.ExportRSAPublicKey(),
        //            Base64FormattingOptions.InsertLineBreaks));
        //    builder.AppendLine(PemDecoder.GetEnd(PemTypes.RSA_PUBLIC_KEY));
        //    return builder.ToString();
        //}

        /// <summary>
        /// public key certificate export in pem format
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns>CERTIFICATE pem export</returns>
        public string PemExportPublicKeyCertificate(X509Certificate2 certificate)
        {
            var publicKeyCrt = ExportCertificatePublicKey(certificate);
            var deviceVerifyPublicKeyBytes = publicKeyCrt.Export(X509ContentType.Cert);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(PemDecoder.GetBegin(PemTypes.CERTIFICATE));
            builder.AppendLine(Convert.ToBase64String(deviceVerifyPublicKeyBytes,
                    Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine(PemDecoder.GetEnd(PemTypes.CERTIFICATE));
            return builder.ToString();
        }

        /// <summary>
        /// https://8gwifi.org/PemParserFunctions.jsp
        /// </summary>
        /// <param name="pemCertificate">A pem string type CERTIFICATE with, without private key</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public X509Certificate2 PemImportCertificate(string pemCertificate, string password = null)
        {
            var certBytes = PemDecoder.DecodeSection(pemCertificate, PemTypes.CERTIFICATE);

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

        /// <summary>
        /// Supported EC, RSA
        /// </summary>
        /// <param name="pemCertificate"></param>
        /// <returns></returns>
        public AsymmetricAlgorithm PemImportPrivateKey(string pemCertificate)
        {
            return PemDecoder.LoadPrivateKey(pemCertificate);
        }

        public X509Certificate2 CreateCertificateWithPrivateKey(
            X509Certificate2 certificate,
            AsymmetricAlgorithm privateKey,
            string password = null)
        {
            return PemDecoder.CreateCertificateWithPrivateKey(certificate, privateKey, password);
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
