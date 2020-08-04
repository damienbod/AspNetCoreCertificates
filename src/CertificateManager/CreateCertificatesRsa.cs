using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    public class CreateCertificatesRsa
    {
        private readonly CreateCertificates _createCertificates;

        public CreateCertificatesRsa(CreateCertificates createCertificates)
        {
            _createCertificates = createCertificates;
        }

        /// <summary>
        /// creates a development certificate
        /// </summary>
        /// <param name="dnsName">DNS name ie localhost etc</param>
        /// <param name="validityPeriodInYears">valid time in years</param>
        /// <param name="keySize">1024 2048 4096</param>
        /// <returns></returns>
        public X509Certificate2 CreateDevelopmentCertificate(string dnsName, int validityPeriodInYears, int keySize = 1024)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 3,
                Critical = true
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    dnsName,
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign
                | X509KeyUsageFlags.DigitalSignature
                | X509KeyUsageFlags.KeyEncipherment
                | X509KeyUsageFlags.CrlSign
                | X509KeyUsageFlags.DataEncipherment
                | X509KeyUsageFlags.NonRepudiation
                | X509KeyUsageFlags.KeyAgreement;

            var enhancedKeyUsages = new OidCollection {
                OidLookup.ServerAuthentication,
                OidLookup.ClientAuthentication,
                OidLookup.CodeSigning,
                OidLookup.SecureEmail,
                OidLookup.TimeStamping
            };

            var certificate = _createCertificates.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = dnsName },
                basicConstraints,
                new ValidityPeriod { ValidFrom = DateTimeOffset.UtcNow, 
                    ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears) },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                {
                    KeySize = keySize
                });

            return certificate;
        }
    }
}
