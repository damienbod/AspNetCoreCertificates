using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;

namespace CertificateManagerTests
{
    public class RsaKeySizeTests
    {
        [Fact]
        public void CreateChainedCertificatesRsaKeySizeTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var cc = serviceProvider.GetService<CreateCertificates>();
            var cert2048 = CreateRsaCertificate(cc, 2048);
            var cert4096 = CreateRsaCertificate(cc, 4096);

            var chained1024 = CreateRsaCertificateChained(cc, 1024, cert2048);
            var chained4096 = CreateRsaCertificateChained(cc, 4096, cert2048);
            Assert.Equal(1024, chained1024.PrivateKey.KeySize);
            Assert.Equal(4096, chained4096.PrivateKey.KeySize);
        }

        [Fact]
        public void CreateCertificatesRsaKeySizeTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var ccRsa = serviceProvider.GetService<CreateCertificatesRsa>();
            var cert2048 = ccRsa.CreateDevelopmentCertificate("localhost", 2, 2048);
            Assert.Equal(2048, cert2048.PrivateKey.KeySize);

            var cert1024 = ccRsa.CreateDevelopmentCertificate("localhost", 2);
            Assert.Equal(1024, cert1024.PrivateKey.KeySize);
        }

        [Fact]
        public void RsaKeySizeTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var cc = serviceProvider.GetService<CreateCertificates>();

            var cert2048 = CreateRsaCertificate(cc, 2048);
            Assert.Equal(2048, cert2048.PrivateKey.KeySize);

            var cert4096= CreateRsaCertificate(cc, 4096);
            Assert.Equal(4096, cert4096.PrivateKey.KeySize);
        }

        public static X509Certificate2 CreateRsaCertificate(CreateCertificates createCertificates, int keySize)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 2,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    "localhost",
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign
               | X509KeyUsageFlags.DigitalSignature
               | X509KeyUsageFlags.KeyEncipherment
               | X509KeyUsageFlags.CrlSign
               | X509KeyUsageFlags.DataEncipherment
               | X509KeyUsageFlags.NonRepudiation
               | X509KeyUsageFlags.KeyAgreement;

            // only if mtls is used
            var enhancedKeyUsages = new OidCollection
            {
                new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
                new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
                //new Oid("1.3.6.1.5.5.7.3.3"),  // Code signing 
                //new Oid("1.3.6.1.5.5.7.3.4"),  // Email
                //new Oid("1.3.6.1.5.5.7.3.8")   // Timestamping  
            };

            var certificate = createCertificates.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = "localhost" },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(1)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                {
                    KeySize = keySize
                });

            return certificate;
        }

        public static X509Certificate2 CreateRsaCertificateChained(CreateCertificates createCertificates, int keySize, X509Certificate2 parentCert)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = false,
                HasPathLengthConstraint = false,
                PathLengthConstraint = 0,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    "localhost",
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

            // only if mtls is used
            var enhancedKeyUsages = new OidCollection
            {
                new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
                new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
                //new Oid("1.3.6.1.5.5.7.3.3"),  // Code signing 
                //new Oid("1.3.6.1.5.5.7.3.4"),  // Email
                //new Oid("1.3.6.1.5.5.7.3.8")   // Timestamping  
            };

            var certificate = createCertificates.NewRsaChainedCertificate(
                new DistinguishedName { CommonName = "localhost" },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(1)
                },
                subjectAlternativeName,
                parentCert,
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
