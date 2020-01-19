using CertificateManager.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CreateChainedCertsConsoleDemo
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

        public static SubjectAlternativeName SubjectAlternativeName = new SubjectAlternativeName
        {
            DnsName = new List<string>
            {
                "localhost"
            }
        };

        public static X509KeyUsageFlags X509KeyUsageFlags = 
             X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;
    }
}
