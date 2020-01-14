using CertificatesCreation.Models;
using System;

namespace CertificatesCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create Root Certificate");

            DistinguishedName distinguishedNameRoot = new DistinguishedName
            {
                CommonName = "localhost",
                Country = "CH",
                Locality = "CH",
                Organisation = "damienbod",
                OrganisationUnit = "developement"
            };

            BasicConstraints basicConstraintsRoot = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 3,
                Critical = true
            };

            ValidityPeriod validityPeriodRoot = new ValidityPeriod
            {
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddYears(10)
            };

            SubjectAlternativeName subjectAlternativeNameRoot = new SubjectAlternativeName
            {
                Email = "damienbod@damienbod.ch"
            };
            subjectAlternativeNameRoot.DnsName.Add("localhost");

            RootCertificate rcCreator = new RootCertificate();

            var rootCert = rcCreator.CreateRootCertificate(
                distinguishedNameRoot, 
                basicConstraintsRoot,
                validityPeriodRoot,
                subjectAlternativeNameRoot);
            Console.WriteLine($"Created Root Certificate {rootCert.SubjectName}");

            DistinguishedName distinguishedNameIntermediate = new DistinguishedName
            {
                CommonName = "localhost",
                Country = "CH",
                Locality = "CH",
                Organisation = "damienbod",
                OrganisationUnit = "region europe"
            };

            BasicConstraints basicConstraintsIntermediate = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 2,
                Critical = true
            };

            ValidityPeriod validityPeriodIntermediate = new ValidityPeriod
            {
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddYears(9)
            };

            SubjectAlternativeName subjectAlternativeNameIntermediate = new SubjectAlternativeName
            {
                Email = "damienbod@damienbod.ch"
            };
            subjectAlternativeNameRoot.DnsName.Add("localhost");

            var icCreator = new IntermediateCertificate();

            var intermediateCertificate = icCreator.CreateIntermediateCertificate(
                distinguishedNameIntermediate,
                basicConstraintsIntermediate,
                validityPeriodIntermediate,
                subjectAlternativeNameIntermediate,
                rootCert);

            

            Console.WriteLine($"Created Intermediate Certificate {intermediateCertificate.SubjectName}");
        }
    }
}
