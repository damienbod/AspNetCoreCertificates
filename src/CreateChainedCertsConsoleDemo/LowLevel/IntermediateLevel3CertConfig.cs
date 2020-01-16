using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateChainedCertsConsoleDemo
{
    public static class IntermediateLevel3CertConfig
    {
        public static DistinguishedName DistinguishedName = new DistinguishedName
        {
            CommonName = "localhost",
            Country = "DE",
            Locality = "DE",
            Organisation = "damienbod",
            OrganisationUnit = "region germany"
        };

        public static BasicConstraints BasicConstraints = new BasicConstraints
        {
            CertificateAuthority = true,
            HasPathLengthConstraint = true,
            PathLengthConstraint = 1,
            Critical = true
        };

        public static ValidityPeriod ValidityPeriod = new ValidityPeriod
        {
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddYears(10)
        };

        public static SubjectAlternativeName SubjectAlternativeName = new SubjectAlternativeName
        {
            DnsName = new List<string>
            {
                "localhost"
            }
        };
    }
}
