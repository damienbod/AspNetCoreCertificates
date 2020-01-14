using CertificateManager.Models;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
    public class RootCertificate
    {
        private readonly CertificateUtility _certificateUtility;

        public RootCertificate(CertificateUtility certificateUtility)
        {
            _certificateUtility = certificateUtility;
        }

        public X509Certificate2 CreateRootCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            ValidityPeriod validityPeriod,
            SubjectAlternativeName subjectAlternativeName)
        {
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
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, true));

                _certificateUtility.AddSubjectAlternativeName(request, subjectAlternativeName);

                // Enhanced key usages
                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection {
                            new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                            new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
                        },
                        false));

                // add this subject key identifier
                request.CertificateExtensions.Add(
                    new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

                var notbefore = validityPeriod.ValidFrom.AddDays(-1);
                var notafter = validityPeriod.ValidTo;
                X509Certificate2 generatedCertificate = request.CreateSelfSigned(notbefore, notafter);

                return generatedCertificate;
            }
        }

    }
}
