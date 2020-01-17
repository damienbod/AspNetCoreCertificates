using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateManager
{
    public class CreateCertificates
    {
        private readonly IntermediateCertificate _intermediateCertificate;
        private readonly RootCertificate _rootCertificate;
        private readonly DeviceCertificate _deviceCertificate;

        public CreateCertificates(
            IntermediateCertificate intermediateCertificate,
            RootCertificate rootCertificate,
            DeviceCertificate deviceCertificate)
        {
            _intermediateCertificate = intermediateCertificate;
            _rootCertificate = rootCertificate;
            _deviceCertificate = deviceCertificate;
        }

        /// <summary>
        /// Create a Self signed certificate with all options which can also be used as a root certificate
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="subjectAlternativeName">SAN but only DnsNames can be added as a list + Email property</param>
        /// <param name="enhancedKeyUsages">Defines how the certificate key can be used. 
        ///  new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
        ///  new Oid("1.3.6.1.5.5.7.3.2")  // TLS Client auth
        ///  new Oid("1.3.6.1.5.5.7.3.3")  // Code signing 
        ///  new Oid("1.3.6.1.5.5.7.3.4")  // Email
        ///  new Oid("1.3.6.1.5.5.7.3.8")  // Timestamping  
        /// </param>
        /// <param name="x509KeyUsageFlags">Defines how the certificate key can be used. 
        ///  None             No key usage parameters.
        ///  EncipherOnly     The key can be used for encryption only.
        ///  CrlSign          The key can be used to sign a certificate revocation list (CRL).
        ///  KeyCertSign      The key can be used to sign certificates.
        ///  KeyAgreement     The key can be used to determine key agreement, such as a key created using the Diffie-Hellman key agreement algorithm.
        ///  DataEncipherment The key can be used for data encryption.
        ///  KeyEncipherment  The key can be used for key encryption.
        ///  NonRepudiation   The key can be used for authentication.
        ///  DecipherOnly     The key can be used for decryption only.
        ///  </param>
        /// <returns>Self signed certificate</returns>
        public X509Certificate2 NewSelfSignedCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags)
        {
            var rootCert = _rootCertificate.CreateRootCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return rootCert;
        }

        public X509Certificate2 NewChainedCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            X509Certificate2 parentCertificate,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags)
        {
            var intermediateCert = _intermediateCertificate.CreateIntermediateCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                parentCertificate,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return intermediateCert;
        }
    }
}
