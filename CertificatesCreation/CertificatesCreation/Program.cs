using CertificatesCreation.Models;
using System;

namespace CertificatesCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create Root Certificate");

            DistinguishedName distinguishedName = new DistinguishedName
            {
                CommonName = "localhost",
                Country = "CH",
                Locality = "CH",
                Organisation = "damienbod",
                OrganisationUnit = "developement"
            };

            BasicConstraints basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 4,
                Critical = true
            };

            ValidityPeriod validityPeriod = new ValidityPeriod
            {
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddYears(1)
            };

            RootCertificate rcCreator = new RootCertificate();

            var rootCert = rcCreator.CreateRootCertificate(
                distinguishedName, 
                basicConstraints,
                validityPeriod);
            Console.WriteLine($"Created Root Certificate {rootCert.SubjectName}");
        }
    }
}
