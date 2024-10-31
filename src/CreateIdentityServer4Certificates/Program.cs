﻿using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreateIdentityServerCertificates;

class Program
{
    static CreateCertificates _cc;
    static void Main(string[] args)
    {
        var sp = new ServiceCollection()
           .AddCertificateManager()
           .BuildServiceProvider();

        _cc = sp.GetService<CreateCertificates>();

        var rsaCert = CreateRsaCertificate("localhost", 10);
        var ecdsaCert = CreateECDsaCertificate("localhost", 10);

        string password = "1234";
        var iec = sp.GetService<ImportExportCertificate>();

        var rsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(password, rsaCert);
        File.WriteAllBytes("cert_rsa512.pfx", rsaCertPfxBytes);

        var ecdsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(password, ecdsaCert);
        File.WriteAllBytes("cert_ecdsa384.pfx", ecdsaCertPfxBytes);

        Console.WriteLine("created");
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

        // only if certification authentication is used
        var enhancedKeyUsages = new OidCollection
        {
            OidLookup.ClientAuthentication,
            OidLookup.ServerAuthentication 
            // OidLookup.CodeSigning,
            // OidLookup.SecureEmail,
            // OidLookup.TimeStamping  
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
                KeySize = 2048,
                HashAlgorithmName = HashAlgorithmName.SHA512
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

        // only if certification authentication is used
        var enhancedKeyUsages = new OidCollection {
            OidLookup.ClientAuthentication,
            OidLookup.ServerAuthentication 
            // OidLookup.CodeSigning,
            // OidLookup.SecureEmail,
            // OidLookup.TimeStamping 
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
                KeySize = 384,
                HashAlgorithmName = HashAlgorithmName.SHA384
            });

        return certificate;
    }
}
