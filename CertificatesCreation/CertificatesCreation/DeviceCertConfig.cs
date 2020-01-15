using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CertificatesCreation
{
    public static class DeviceCertConfig
    {
        public static DistinguishedName DistinguishedName = new DistinguishedName
        {
            CommonName = "localhost",
            Country = "CH",
            Locality = "CH",
            Organisation = "firma x",
            OrganisationUnit = "skills"
        };

        public static BasicConstraints BasicConstraints = new BasicConstraints
        {
            CertificateAuthority = false,
            HasPathLengthConstraint = false,
            PathLengthConstraint = 0,
            Critical = true
        };

        public static ValidityPeriod ValidityPeriod = new ValidityPeriod
        {
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddYears(10)
        };

        public static SubjectAlternativeName SubjectAlternativeName = GetSubjectAlternativeName();

        private static SubjectAlternativeName GetSubjectAlternativeName()
        {
            var subjectAlternativeName = new SubjectAlternativeName
            {
                // Email = "damienbod@damienbod.ch"
            };

            subjectAlternativeName.DnsName.Add("localhost");

            return subjectAlternativeName;
        }
    }
}
