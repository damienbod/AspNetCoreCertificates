using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateChainedCertsConsoleDemo
{
    public static class RootCertConfig
    {
        public static DistinguishedName DistinguishedName = new DistinguishedName
        {
            CommonName = "localhost",
            Country = "CH",
            Locality = "CH",
            Organisation = "damienbod",
            OrganisationUnit = "developement"
        };

        public static BasicConstraints BasicConstraints = new BasicConstraints
        {
            CertificateAuthority = true,
            HasPathLengthConstraint = true,
            PathLengthConstraint = 3,
            Critical = true
        };

        public static ValidityPeriod ValidityPeriod = new ValidityPeriod
        {
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddYears(10)
        };

        public static SubjectAlternativeName SubjectAlternativeName = new SubjectAlternativeName
        {
            Email = "damienbod@damienbod.ch",
            DnsName = new List<string>
            {
                "localhost",
                "test.damienbod.ch"
            }
        };
    }
}
