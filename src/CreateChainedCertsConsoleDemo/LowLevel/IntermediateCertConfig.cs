using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CreateChainedCertsConsoleDemo
{
    public static class IntermediateCertConfig
    {
        public static DistinguishedName DistinguishedName = new DistinguishedName
        {
            CommonName = "localhost",
            Country = "CH",
            Locality = "CH",
            Organisation = "damienbod",
            OrganisationUnit = "region europe"
        };

        public static BasicConstraints BasicConstraints = new BasicConstraints
        {
            CertificateAuthority = true,
            HasPathLengthConstraint = true,
            PathLengthConstraint = 2,
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

        // Only X509KeyUsageFlags.KeyCertSign required for client server auth
        public static X509KeyUsageFlags X509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature
               | X509KeyUsageFlags.KeyEncipherment
               | X509KeyUsageFlags.KeyCertSign;
    }
}
