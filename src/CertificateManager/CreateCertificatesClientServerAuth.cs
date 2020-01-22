using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    public class CreateCertificatesClientServerAuth
    {
        private readonly CreateCertificates _createCertificates;

        public CreateCertificatesClientServerAuth(CreateCertificates createCertificates)
        {
            _createCertificates = createCertificates;
        }

        /// <summary>
        /// Create a root self signed certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="pathLengthConstraint">path length for the amount of chained certificates</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <returns>X509Certificate2 root self signed certificate</returns>
        public X509Certificate2 NewRootCertificate(
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

            var rootCert = _createCertificates.NewSelfSignedCertificate(
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
        public X509Certificate2 NewIntermediateChainedCertificate(
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

            var intermediateCert = _createCertificates.NewChainedCertificate(
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
        /// 
        /// The device certificate (also called a leaf certificate) must have the Subject Name set to the Device ID that was used when registering the IoT device in the Azure IoT Hub. This setting is required for authentication.
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <param name="parentCertificateAuthority"> Parent cert to create the chain from</param>
        /// <returns>X509Certificate2 device chained certificate</returns>
        public X509Certificate2 NewDeviceChainedCertificate(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            };

            return NewDeviceChainedCertificate(distinguishedName,
                validityPeriod, dnsName, enhancedKeyUsages, parentCertificateAuthority);
        }

        public X509Certificate2 NewDeviceVerificationCertificate(
           string deviceVerification,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
            };

            var distinguishedName = new DistinguishedName
            {
                CommonName = deviceVerification
            };

            var validityPeriod = new ValidityPeriod
            {
                ValidFrom = DateTimeOffset.UtcNow,
                ValidTo = DateTimeOffset.UtcNow.AddDays(2)
            };

            return NewDeviceChainedCertificate(distinguishedName,
                validityPeriod, "verify", enhancedKeyUsages, parentCertificateAuthority);
        }
        /// <summary>
        /// Create an server chained certificate for Client and Server TLS Auth
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="dnsName">Dns name use the certificate validation</param>
        /// <param name="parentCertificateAuthority"> Parent cert to create the chain from</param>
        /// <returns>X509Certificate2 server chained certificate</returns>
        public X509Certificate2 NewClientChainedCertificate(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
            };

            return NewDeviceChainedCertificate(distinguishedName,
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
        public X509Certificate2 NewServerChainedCertificate(
           DistinguishedName distinguishedName,
           ValidityPeriod validityPeriod,
           string dnsName,
           X509Certificate2 parentCertificateAuthority)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            };

            return NewDeviceChainedCertificate(distinguishedName,
                validityPeriod, dnsName, enhancedKeyUsages, parentCertificateAuthority);
        }

        public X509Certificate2 NewServerSelfSignedCertificate(
            DistinguishedName distinguishedName,
            ValidityPeriod validityPeriod,
            string dnsName)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.1"), // TLS Server auth
            };

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

            var clientCertSelfSigned = _createCertificates.NewSelfSignedCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return clientCertSelfSigned;
        }
        public X509Certificate2 NewClientSelfSignedCertificate(
            DistinguishedName distinguishedName,
            ValidityPeriod validityPeriod,
            string dnsName)
        {
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
            };

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

            var clientCertSelfSigned = _createCertificates.NewSelfSignedCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags);

            return clientCertSelfSigned;
        }

        private X509Certificate2 NewDeviceChainedCertificate(
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

            var deviceCert = _createCertificates.NewChainedCertificate(
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
