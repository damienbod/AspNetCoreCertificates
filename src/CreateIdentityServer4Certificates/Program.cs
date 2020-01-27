using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreateIdentityServer4Certificates
{
    class Program
    {
        static CreateCertificates _cc;
        static void Main(string[] args)
        {
            var sp = new ServiceCollection()
               .AddCertificateManager()
               .BuildServiceProvider();

            _cc = sp.GetService<CreateCertificates>();

            var rsaCert = CreateRsaCertificate("locahost", 10);
            var ecdsaCert = CreateECDsaCertificate("locahost", 10);

            string password = "1234";
            var iec = sp.GetService<ImportExportCertificate>();

            var rsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(password, rsaCert);
            File.WriteAllBytes("rsaCert.pfx", rsaCertPfxBytes);

            var ecdsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(password, ecdsaCert);
            File.WriteAllBytes("ecdsaCert.pfx", ecdsaCertPfxBytes);

            Console.WriteLine("");
        }

        public static X509Certificate2 CreateRsaCertificate(string dnsName, int validityPeriodInYears)
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
                    dnsName,
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

            var certificate = _cc.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = dnsName },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                { 
                    KeySize = 2048
                });

            return certificate;
        }

        public static X509Certificate2 CreateECDsaCertificate(string dnsName, int validityPeriodInYears)
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
                    dnsName,
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

            // only if mtls is used
            var enhancedKeyUsages = new OidCollection {
                new Oid("1.3.6.1.5.5.7.3.1"),  // TLS Server auth
                new Oid("1.3.6.1.5.5.7.3.2"),  // TLS Client auth
                //new Oid("1.3.6.1.5.5.7.3.3"),  // Code signing 
                //new Oid("1.3.6.1.5.5.7.3.4"),  // Email
                //new Oid("1.3.6.1.5.5.7.3.8")   // Timestamping  
            };

            var certificate = _cc.NewECDsaSelfSignedCertificate(
                new DistinguishedName { CommonName = dnsName },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new ECDsaConfiguration
                {
                    //KeySize = 256
                });

            return certificate;
        }
    }
}
