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
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags)
        {
            using var ecdsa = ECDsa.Create("ECDsa");

            ecdsa.KeySize = 256;
            var request = new CertificateRequest(
                _certificateUtility.CreateIssuerOrSubject(distinguishedName),
                ecdsa,
                HashAlgorithmName.SHA256);

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
