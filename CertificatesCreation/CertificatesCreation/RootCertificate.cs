using CertificatesCreation.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificatesCreation
{
    public class RootCertificate
    {
        public X509Certificate2 CreateRootCertificate(
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints)
        {
            using (var ecdsa = ECDsa.Create("ECDsa"))
            {
                ecdsa.KeySize = 256;
                var request = new CertificateRequest(
                    CreateIssuerOrSubject(distinguishedName),
                    ecdsa,
                    HashAlgorithmName.SHA256);

                // set basic certificate contraints
                AddBasicConstraints(request, basicConstraints);

                // key usage: Digital Signature and Key Encipherment
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, true));

                var sanBuilder = new SubjectAlternativeNameBuilder();
                sanBuilder.AddDnsName(distinguishedName.CommonName);
                var sanExtension = sanBuilder.Build();
                request.CertificateExtensions.Add(sanExtension);

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

                // certificate expiry: Valid from Yesterday to Now+365 days
                var notbefore = DateTimeOffset.UtcNow.AddDays(-1);
                var notafter = DateTimeOffset.UtcNow.AddDays(365);
                
                // cert serial is the epoch/unix timestamp
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
  
                X509Certificate2 generatedCertificate = request.CreateSelfSigned(notbefore, notafter);

                return generatedCertificate;
            }
        }

        /// <summary>
        /// OID 2.5.29.19 basicConstraints
        /// 
        /// This extension is used during the certificate chain verification process to identify CA 
        /// certificates and to apply certificate chain path length constraints. 
        /// The CA component should be set to true for all CA certificates. 
        /// PKIX recommends that this extension should not appear in end-entity certificates.
        /// 
        /// If the pathLenConstraint component is present, its value must be greater than the number
        /// of CA certificates that have been processed so far, starting with the end-entity certificate 
        /// and moving up the chain.If pathLenConstraint is omitted, then all of the higher level CA certificates
        /// in the chain must not include this component when the extension is present.
        /// </summary>
        private void AddBasicConstraints(CertificateRequest request, BasicConstraints basicConstraints)
        {
            request.CertificateExtensions.Add(
               new X509BasicConstraintsExtension(
                   basicConstraints.CertificateAuthority,
                   basicConstraints.HasPathLengthConstraint,
                   basicConstraints.PathLengthConstraint,
                   basicConstraints.Critical));
        }

        private string CreateIssuerOrSubject(DistinguishedName distinguishedName)
        {
            var sb = new StringBuilder($"CN={distinguishedName.CommonName}, C={distinguishedName.Country}");

            if(!string.IsNullOrEmpty(distinguishedName.Organisation))
            {
                sb.Append($", O={ distinguishedName.Organisation}");
            }

            if (!string.IsNullOrEmpty(distinguishedName.OrganisationUnit))
            {
                sb.Append($", OU={ distinguishedName.OrganisationUnit}");
            }

            if (!string.IsNullOrEmpty(distinguishedName.Locality))
            {
                sb.Append($", L={ distinguishedName.Locality}");
            }

            if (!string.IsNullOrEmpty(distinguishedName.StateProvince))
            {
                sb.Append($", ST={ distinguishedName.StateProvince}");
            }

            return sb.ToString();
        }
    }
}
