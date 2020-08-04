using CertificateManager.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    public class CreateCertificates
    {
        private readonly CertificateUtility _certificateUtility;

        public CreateCertificates(CertificateUtility certificateUtility)
        {
            _certificateUtility = certificateUtility;
        }

        /// <summary>
        /// Create a Self signed certificate with all options which can also be used as a root certificate
        /// </summary>
        /// <param name="distinguishedName">Distinguished Name used for the subject and the issuer properties</param>
        /// <param name="validityPeriod">Valid from, Valid to certificate properties</param>
        /// <param name="subjectAlternativeName">SAN but only DnsNames can be added as a list + Email property</param>
        /// <param name="enhancedKeyUsages">Defines how the certificate key can be used. 
        /// OidLookup.ServerAuthentication,
        /// OidLookup.ClientAuthentication,
        /// OidLookup.CodeSigning,
        /// OidLookup.SecureEmail,
        /// OidLookup.TimeStamping
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
        public X509Certificate2 NewECDsaSelfSignedCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            ECDsaConfiguration eCDsaConfiguration)
        {
            using var ecdsa = ECDsa.Create("ECDsa");
            ecdsa.KeySize = eCDsaConfiguration.KeySize;
            var request = new CertificateRequest(
                _certificateUtility.CreateIssuerOrSubject(distinguishedName),
                ecdsa,
                eCDsaConfiguration.HashAlgorithmName);

            return NewECDsaSelfSignedCertificate(basicConstraints,
                                                 validityPeriod,
                                                 subjectAlternativeName,
                                                 enhancedKeyUsages,
                                                 x509KeyUsageFlags,
                                                 request);
        }

        public X509Certificate2 NewECDsaSelfSignedCertificate(
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateRequest request) 
        {

            X509Certificate2 generatedCertificate = SelfSignedConfiguration(
                basicConstraints, 
                validityPeriod, 
                subjectAlternativeName, 
                enhancedKeyUsages, 
                x509KeyUsageFlags, 
                request);

            return generatedCertificate;
        }

        public X509Certificate2 NewRsaSelfSignedCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            RsaConfiguration rsaConfiguration)
        {
            using var rsa = RSA.Create(rsaConfiguration.KeySize); // 1024, 2048 or 4096
            var request = new CertificateRequest(
                _certificateUtility.CreateIssuerOrSubject(distinguishedName),
                rsa,
                rsaConfiguration.HashAlgorithmName, 
                rsaConfiguration.RSASignaturePadding);

            return NewRsaSelfSignedCertificate(basicConstraints,
                                               validityPeriod,
                                               subjectAlternativeName,
                                               enhancedKeyUsages,
                                               x509KeyUsageFlags,
                                               request);
        }

        public X509Certificate2 NewRsaSelfSignedCertificate(
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateRequest request)
        {

            X509Certificate2 generatedCertificate = SelfSignedConfiguration(
                basicConstraints, 
                validityPeriod, 
                subjectAlternativeName, 
                enhancedKeyUsages, 
                x509KeyUsageFlags, 
                request);

            return generatedCertificate;
        }

        public X509Certificate2 NewRsaChainedCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            X509Certificate2 signingCertificate,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            RsaConfiguration rsaConfiguration)
        {
            using var rsa = RSA.Create(rsaConfiguration.KeySize);
            var request = new CertificateRequest(
                _certificateUtility.CreateIssuerOrSubject(distinguishedName),
                rsa,
                rsaConfiguration.HashAlgorithmName,
                rsaConfiguration.RSASignaturePadding);

            return NewRsaChainedCertificate(basicConstraints,
                                            validityPeriod,
                                            subjectAlternativeName,
                                            signingCertificate,
                                            enhancedKeyUsages,
                                            x509KeyUsageFlags,
                                            request,
                                            rsa);
        }

        public X509Certificate2 NewRsaChainedCertificate(
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            X509Certificate2 signingCertificate,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateRequest request,
            RSA rsa)
        {
            if (signingCertificate == null)
            {
                throw new ArgumentNullException(nameof(signingCertificate));
            }
            if (!signingCertificate.HasPrivateKey)
            {
                throw new Exception("Signing cert must have private key");
            }

            X509Certificate2 cert = ChainedConfiguration(
                basicConstraints, 
                validityPeriod, 
                subjectAlternativeName, 
                signingCertificate, 
                enhancedKeyUsages, 
                x509KeyUsageFlags, 
                request);

            if (rsa == null)
            {
                return cert;
            }
            else
            {
                return cert.CopyWithPrivateKey(rsa);
            }
        }

        public X509Certificate2 NewECDsaChainedCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            X509Certificate2 signingCertificate,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            ECDsaConfiguration eCDsaConfiguration)
        {
            using var ecdsa = ECDsa.Create("ECDsa");
            ecdsa.KeySize = eCDsaConfiguration.KeySize;
            var request = new CertificateRequest(
                _certificateUtility.CreateIssuerOrSubject(distinguishedName),
                ecdsa,
                eCDsaConfiguration.HashAlgorithmName);

            return NewECDsaChainedCertificate(basicConstraints,
                                              validityPeriod,
                                              subjectAlternativeName,
                                              signingCertificate,
                                              enhancedKeyUsages,
                                              x509KeyUsageFlags,
                                              request,
                                              ecdsa);
        }

        public X509Certificate2 NewECDsaChainedCertificate(
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            X509Certificate2 signingCertificate,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateRequest request,
            ECDsa ecdsa)
        {
            if (signingCertificate == null)
            {
                throw new ArgumentNullException(nameof(signingCertificate));
            }
            if (!signingCertificate.HasPrivateKey)
            {
                throw new Exception("Signing cert must have private key");
            }

            X509Certificate2 cert = ChainedConfiguration(
                basicConstraints, 
                validityPeriod, 
                subjectAlternativeName, 
                signingCertificate, 
                enhancedKeyUsages, 
                x509KeyUsageFlags, 
                request);
            if (ecdsa == null)
            {
                return cert;
            } 
            else
            {
                return cert.CopyWithPrivateKey(ecdsa);
            }
        }

        private X509Certificate2 ChainedConfiguration(BasicConstraints basicConstraints, ValidityPeriod validityPeriod, SubjectAlternativeName subjectAlternativeName, X509Certificate2 signingCertificate, OidCollection enhancedKeyUsages, X509KeyUsageFlags x509KeyUsageFlags, CertificateRequest request)
        {
            _certificateUtility.AddBasicConstraints(request, basicConstraints);
            _certificateUtility.AddExtendedKeyUsages(request, x509KeyUsageFlags);

            // set the AuthorityKeyIdentifier. There is no built-in 
            // support, so it needs to be copied from the Subject Key 
            // Identifier of the signing certificate and massaged slightly.
            // AuthorityKeyIdentifier is "KeyID=<subject key identifier>"
            foreach (var item in signingCertificate.Extensions)
            {
                if (item.Oid.Value == "2.5.29.14") //  "Subject Key Identifier"
                {
                    var issuerSubjectKey = item.RawData;
                    //var issuerSubjectKey = signingCertificate.Extensions["Subject Key Identifier"].RawData;
                    var segment = new ArraySegment<byte>(issuerSubjectKey, 2, issuerSubjectKey.Length - 2);
                    var authorityKeyIdentifier = new byte[segment.Count + 4];
                    // "KeyID" bytes
                    authorityKeyIdentifier[0] = 0x30;
                    authorityKeyIdentifier[1] = 0x16;
                    authorityKeyIdentifier[2] = 0x80;
                    authorityKeyIdentifier[3] = 0x14;
                    segment.CopyTo(authorityKeyIdentifier, 4);
                    request.CertificateExtensions.Add(new X509Extension("2.5.29.35", authorityKeyIdentifier, false));
                    break;
                }
            }
            
            _certificateUtility.AddSubjectAlternativeName(request, subjectAlternativeName);

            // Enhanced key usages
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(enhancedKeyUsages, false));

            // add this subject key identifier
            request.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            // certificate expiry: Valid from Yesterday to Now+365 days
            // Unless the signing cert's validity is less. It's not possible
            // to create a cert with longer validity than the signing cert.
            var notbefore = validityPeriod.ValidFrom.AddDays(-1);
            if (notbefore < signingCertificate.NotBefore)
            {
                notbefore = new DateTimeOffset(signingCertificate.NotBefore);
            }

            var notafter = validityPeriod.ValidTo;
            if (notafter > signingCertificate.NotAfter)
            {
                notafter = new DateTimeOffset(signingCertificate.NotAfter);
            }

            // cert serial is the epoch/unix timestamp
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
            var serial = BitConverter.GetBytes(unixTime);
            var cert = request.Create(
                            signingCertificate,
                            notbefore,
                            notafter,
                            serial);
            return cert;
        }

        private X509Certificate2 SelfSignedConfiguration(BasicConstraints basicConstraints, ValidityPeriod validityPeriod, SubjectAlternativeName subjectAlternativeName, OidCollection enhancedKeyUsages, X509KeyUsageFlags x509KeyUsageFlags, CertificateRequest request)
        {
            _certificateUtility.AddBasicConstraints(request, basicConstraints);
            _certificateUtility.AddExtendedKeyUsages(request, x509KeyUsageFlags);
            _certificateUtility.AddSubjectAlternativeName(request, subjectAlternativeName);

            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(enhancedKeyUsages, false));

            request.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            var notbefore = validityPeriod.ValidFrom.AddDays(-1);
            var notafter = validityPeriod.ValidTo;
            X509Certificate2 generatedCertificate = request.CreateSelfSigned(notbefore, notafter);
            return generatedCertificate;
        }
    }
}
