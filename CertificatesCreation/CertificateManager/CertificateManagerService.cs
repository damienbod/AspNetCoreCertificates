using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateManager
{
    public class CertificateManagerService
    {
        private readonly IntermediateCertificate _intermediateCertificate;
        private readonly RootCertificate _rootCertificate;
        private readonly DeviceCertificate _deviceCertificate;

        public CertificateManagerService(
            IntermediateCertificate intermediateCertificate,
            RootCertificate rootCertificate,
            DeviceCertificate deviceCertificate)
        {
            _intermediateCertificate = intermediateCertificate;
            _rootCertificate = rootCertificate;
            _deviceCertificate = deviceCertificate;
        }

        /// <summary>
        /// Create a root certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="pathLengthConstraint">path length for the amount of chained certificates</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <returns>X509Certificate2 root self signed certificate</returns>
        public X509Certificate2 CreateRootCertificateForClientServerAuth(
            DistinguishedName distinguishedName,
            ValidityPeriod validityPeriod,
            int pathLengthConstraint,
            string dnsName)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            };

            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = pathLengthConstraint,
                Critical = true
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    dnsName,
                }
            };
            var rootCert = _rootCertificate.CreateRootCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                enhancedKeyUsages);

            return rootCert;
        }
    }
}
