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

        public X509Certificate2 CreateDevelopmentCertificate(string dnsName, int validityPeriodInYears)
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

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign;

            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
                new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
                new Oid("1.3.6.1.5.5.7.3.3"),  // Code signing 
                new Oid("1.3.6.1.5.5.7.3.4"),  // Email
                new Oid("1.3.6.1.5.5.7.3.8")   // Timestamping  
            };

            var certificate = _createCertificates.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = dnsName },
                basicConstraints,
                new ValidityPeriod { ValidFrom = DateTimeOffset.UtcNow, 
                    ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears) },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration());

            return certificate;
        }
    }
}
