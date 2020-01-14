using CertificatesCreation.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificatesCreation
{
    internal static class Certificates
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
        internal static void AddBasicConstraints(CertificateRequest request, BasicConstraints basicConstraints)
        {
            request.CertificateExtensions.Add(
               new X509BasicConstraintsExtension(
                   basicConstraints.CertificateAuthority,
                   basicConstraints.HasPathLengthConstraint,
                   basicConstraints.PathLengthConstraint,
                   basicConstraints.Critical));
        }

        internal static void AddSubjectAlternativeName(CertificateRequest request, SubjectAlternativeName subjectAlternativeName)
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            foreach(var dnsName in subjectAlternativeName.DnsName)
            {
                sanBuilder.AddDnsName(dnsName);
            }

            if(!string.IsNullOrEmpty(subjectAlternativeName.Email))
            {
                sanBuilder.AddEmailAddress(subjectAlternativeName.Email);
            }

            var sanExtension = sanBuilder.Build();
            request.CertificateExtensions.Add(sanExtension);
        }
        
        internal static string CreateIssuerOrSubject(DistinguishedName distinguishedName)
        {
            var sb = new StringBuilder($"CN={distinguishedName.CommonName}, C={distinguishedName.Country}");

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
    }
}
