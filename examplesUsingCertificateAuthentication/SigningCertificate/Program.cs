using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;

namespace SigningCertificates
{
    /// <summary>
    /// Purpose of this example is to show how to sign Certificate Signing Request from a device with a root ca certificate.
    /// To make it a bit simple initial files can be created just with openssl command line.
    ///
    /// This example is based on the needs of Azure IoT (Edge) devices and for example CertificateAuthority is set false so that the certificate can be used for device provisioning 
    ///
    /// This example utilises BouncyCastle library to handle reading of CSR, because .Net does not itself have any capability for that
    ///
    /// To begin with create a keypair and certificate for root ca. This will be used to sign the device certificate.
    /// openssl genrsa -aes256 -passout pass:1234 -out ./root.key.pem 4096
    /// openssl req -new -x509 -config ./openssl_root_ca.cnf  -passin pass:1234 -key ./root.key.pem -subj "/CN=Test Root CA" -days 30 -sha256 -extensions "v3_ca" -out ./root.cert.pem
    /// openssl pkcs12 -export -out root.cert.pfx -inkey ./root.key.pem -in ./root.cert.pem 
    ///
    /// Next create device keypair and csr
    /// openssl genrsa -out ./device1.key.pem 2048
    /// openssl req -new -sha256 -config ./openssl_root_ca.cnf -key ./device1.key.pem -subj "/CN=device1" -out ./device1.csr.pem
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createCertificates = serviceProvider.GetService<CreateCertificates>();
            var certificateUtility = serviceProvider.GetService<CertificateUtility>();

            string password = "1234";

            var signingCertificate = new X509Certificate2("root.cert.pfx", password);
            var enhancedKeyUsages = new OidCollection {
                OidLookup.ClientAuthentication,
                OidLookup.ServerAuthentication
            };

            var basicConstraints = new CertificateManager.Models.BasicConstraints
            {
                CertificateAuthority = false,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 3,
                Critical = true
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature
                | X509KeyUsageFlags.KeyEncipherment
                | X509KeyUsageFlags.NonRepudiation;

            // Read in CSR data from a file
            Pkcs10CertificationRequest decodedCsr = (Pkcs10CertificationRequest)new PemReader(new StringReader(File.ReadAllText(args[0]))).ReadObject();
            // Get Common Name (CN) from CSR Subject
            CertificationRequestInfo csrData = decodedCsr.GetCertificationRequestInfo();
            var subjectKeyPairs = csrData.Subject.ToString().Split(',')
                            .Select(x => x.Split('='))
                            .Where(x => x.Length == 2)
                            .ToDictionary(x => x.First(), x => x.Last());
            var commonName = subjectKeyPairs["CN"];
            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    commonName,
                }
            };
            // Get Public key data from CSR and create RSA data based on that
            RsaKeyParameters rsaKeyParams = (RsaKeyParameters)decodedCsr.GetPublicKey();
            var rsaParams = new RSAParameters();
            rsaParams.Modulus = rsaKeyParams.Modulus.ToByteArray();
            rsaParams.Exponent = rsaKeyParams.Exponent.ToByteArray();
            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParams);

            // Create Certificate Request with the data extracted from csr file earlier
            var rsaConfiguration = new RsaConfiguration();
            var request = new CertificateRequest(
                certificateUtility.CreateIssuerOrSubject( new DistinguishedName { CommonName = commonName }),
                rsa,
                rsaConfiguration.HashAlgorithmName,
                rsaConfiguration.RSASignaturePadding);

            // Sign the csr
            var device1Certificate = createCertificates.NewRsaChainedCertificate(
                basicConstraints,
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1) },
                subjectAlternativeName,
                signingCertificate,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                request,
                null);
      
            // Export content of certificates into files
            var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();
            var deviceCertificatePem = importExportCertificate.PemExportPublicKeyCertificate(device1Certificate);
            var signingCertificatePem = importExportCertificate.PemExportPublicKeyCertificate(signingCertificate);
            File.WriteAllText("device1.cert.pem", deviceCertificatePem);
            File.WriteAllText("device1-full-chain.cert.pem", String.Concat(deviceCertificatePem, signingCertificatePem));

            Console.WriteLine("Certificates exported to pem files");
        }
    }
}
