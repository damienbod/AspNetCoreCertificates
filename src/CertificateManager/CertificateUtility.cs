using CertificateManager.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateManager
{
    public class CertificateUtility
    {
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
        public void AddBasicConstraints(CertificateRequest request, BasicConstraints basicConstraints)
        {
            request.CertificateExtensions.Add(
               new X509BasicConstraintsExtension(
                   basicConstraints.CertificateAuthority,
                   basicConstraints.HasPathLengthConstraint,
                   basicConstraints.PathLengthConstraint,
                   basicConstraints.Critical));
        }

        public void AddSubjectAlternativeName(CertificateRequest request, SubjectAlternativeName subjectAlternativeName)
        {
            foreach (var dnsName in subjectAlternativeName.DnsName)
            {
                if (UriHostNameType.Unknown == Uri.CheckHostName(dnsName))
                {
                    throw new ArgumentException("Must be a valid DNS name", nameof(dnsName));
                }
            }

            var sanBuilder = new SubjectAlternativeNameBuilder();
            foreach (var dnsName in subjectAlternativeName.DnsName)
            {
                sanBuilder.AddDnsName(dnsName);
            }

            if (!string.IsNullOrEmpty(subjectAlternativeName.Email))
            {
                sanBuilder.AddEmailAddress(subjectAlternativeName.Email);
            }

            if (subjectAlternativeName.IpAddress != null)
            {
                sanBuilder.AddIpAddress(subjectAlternativeName.IpAddress);
            }

            if (!string.IsNullOrEmpty(subjectAlternativeName.UserPrincipalName))
            {
                sanBuilder.AddUserPrincipalName(subjectAlternativeName.UserPrincipalName);
            }

            if (subjectAlternativeName.Uri != null)
            {
                sanBuilder.AddUri(subjectAlternativeName.Uri);
            }

            var sanExtension = sanBuilder.Build();
            request.CertificateExtensions.Add(sanExtension);
        }

        public string CreateIssuerOrSubject(DistinguishedName distinguishedName)
        {
            if (string.IsNullOrEmpty(distinguishedName.CommonName))
            {
                throw new ArgumentException($"{nameof(distinguishedName.CommonName)} must be a valid name", nameof(distinguishedName.CommonName));
            }

            var sb = new StringBuilder($"CN={distinguishedName.CommonName}");

            if (!string.IsNullOrEmpty(distinguishedName.Country))
            {
                sb.Append($", C={distinguishedName.Country}");
            }

            if (!string.IsNullOrEmpty(distinguishedName.Organisation))
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

        public void AddExtendedKeyUsages(CertificateRequest request, X509KeyUsageFlags x509KeyUsageFlags)
        {
            // root
            // x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature
            //   | X509KeyUsageFlags.KeyEncipherment
            //   | X509KeyUsageFlags.KeyCertSign;

            // device
            // x509KeyUsageFlags = 
            // X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;

            request.CertificateExtensions.Add(new X509KeyUsageExtension(x509KeyUsageFlags, true));
        }
    }
}
