using CertificateManager.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    public class DeviceCertificate
    {
        private readonly CertificateUtility _certificateUtility;

        public DeviceCertificate(CertificateUtility certificateUtility)
        {
            _certificateUtility = certificateUtility;
        }

        public X509Certificate2 CreateDeviceCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName,
            X509Certificate2 signingCertificate,
            OidCollection enhancedKeyUsages)
        {
            if (signingCertificate == null)
            {
                throw new ArgumentNullException(nameof(signingCertificate));
            }
            if (!signingCertificate.HasPrivateKey)
            {
                throw new Exception("Signing cert must have private key");
            }

            using (var ecdsa = ECDsa.Create("ECDsa"))
            {
                ecdsa.KeySize = 256;
                var request = new CertificateRequest(
                    _certificateUtility.CreateIssuerOrSubject(distinguishedName),
                    ecdsa,
                    HashAlgorithmName.SHA256);

                // set basic certificate contraints
                _certificateUtility.AddBasicConstraints(request, basicConstraints);

                // key usage: Digital Signature and Key Encipherment
                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                        true));

                // set the AuthorityKeyIdentifier. There is no built-in 
                // support, so it needs to be copied from the Subject Key 
                // Identifier of the signing certificate and massaged slightly.
                // AuthorityKeyIdentifier is "KeyID=<subject key identifier>"
                var issuerSubjectKey = signingCertificate.Extensions["Subject Key Identifier"].RawData;
                var segment = new ArraySegment<byte>(issuerSubjectKey, 2, issuerSubjectKey.Length - 2);
                var authorityKeyIdentifer = new byte[segment.Count + 4];
                // these bytes define the "KeyID" part of the AuthorityKeyIdentifer
                authorityKeyIdentifer[0] = 0x30;
                authorityKeyIdentifer[1] = 0x16;
                authorityKeyIdentifer[2] = 0x80;
                authorityKeyIdentifer[3] = 0x14;
                segment.CopyTo(authorityKeyIdentifer, 4);
                request.CertificateExtensions.Add(new X509Extension("2.5.29.35", authorityKeyIdentifer, false));

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

                // create and return the generated and signed
                using (var cert = request.Create(
                    signingCertificate,
                    notbefore,
                    notafter,
                    serial))
                {
                    return cert.CopyWithPrivateKey(ecdsa);
                }
            }

        }
    }
}
