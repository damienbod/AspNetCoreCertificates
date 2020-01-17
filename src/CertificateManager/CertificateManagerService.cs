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
        /// Create a root self signed certificate for Client and Server TLS Auth
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

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign;

            var rootCert = _rootCertificate.CreateRootCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return rootCert;
        }

        /// <summary>
        /// Create an intermediate chained certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="pathLengthConstraint">path length for the amount of chained certificates</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <param name="parentCertificateAuthority"> Parent cert to create the chain from</param>
        /// <returns>X509Certificate2 intermediate chained certificate</returns>
        public X509Certificate2 CreateIntermediateCertificateForClientServerAuth(
            DistinguishedName distinguishedName,
            ValidityPeriod validityPeriod,
            int pathLengthConstraint,
            string dnsName,
            X509Certificate2 parentCertificateAuthority)
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

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign;

            var intermediateCert = _intermediateCertificate.CreateIntermediateCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                parentCertificateAuthority,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return intermediateCert;
        }

        /// <summary>
        /// Create an device chained certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <param name="parentCertificateAuthority"> Parent cert to create the chain from</param>
        /// <returns>X509Certificate2 device chained certificate</returns>
        public X509Certificate2 CreateDeviceCertificateForClientServerAuth(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            };

            return CreateDeviceClientServerCertificate(distinguishedName,
                validityPeriod, dnsName, enhancedKeyUsages, parentCertificateAuthority);
        }

        /// <summary>
        /// Create an server chained certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <param name="parentCertificateAuthority"> Parent cert to create the chain from</param>
        /// <returns>X509Certificate2 server chained certificate</returns>
        public X509Certificate2 CreateClientCertificateForClientServerAuth(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
            };

            return CreateDeviceClientServerCertificate(distinguishedName,
                validityPeriod, dnsName, enhancedKeyUsages, parentCertificateAuthority);
        }

        /// <summary>
        /// Create an client chained certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <param name="parentCertificateAuthority"> Parent cert to create the chain from</param>
        /// <returns>X509Certificate2 client chained certificate</returns>
        public X509Certificate2 CreateServerCertificateForClientServerAuth(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            };

            return CreateDeviceClientServerCertificate(distinguishedName,
                validityPeriod, dnsName, enhancedKeyUsages, parentCertificateAuthority);
        }

        private X509Certificate2 CreateDeviceClientServerCertificate(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           OidCollection enhancedKeyUsages, 
           X509Certificate2 parentCertificateAuthority)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = false,
                HasPathLengthConstraint = false,
                PathLengthConstraint = 0,
                Critical = true
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    dnsName,
                }
            };

            var x509KeyUsageFlags =
              X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;

            var deviceCert = _deviceCertificate.CreateDeviceCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                parentCertificateAuthority,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return deviceCert;
        }
    }
}
