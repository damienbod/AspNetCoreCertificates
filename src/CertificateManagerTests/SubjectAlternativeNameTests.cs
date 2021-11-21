using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace CertificateManagerTests
{
    public class SubjectAlternativeNameTests
    {
        [Fact]
        public void SubjectAlternativeNameValid()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createCertificates = serviceProvider.GetService<CreateCertificates>();

            var testCertificate = CreateSubjectAlternativeNameDetails(
                new SubjectAlternativeName
                {
                    DnsName = new List<string> { "testones" , "testtwos" },
                    IpAddress = new IPAddress(2414),
                    Uri = new Uri("https://damienbod.com"),
                    UserPrincipalName = "myNameIsBob",
                    Email = "mick@jones.be"
                }, 
                createCertificates);

            foreach (X509Extension extension in testCertificate.Extensions)
            {
                if(extension.Oid.FriendlyName == "Subject Alternative Name")
                {
                    var asndata = new AsnEncodedData(extension.Oid, extension.RawData);
                    var data = asndata.Format(false);
                    var expected = "DNS Name=testones, DNS Name=testtwos, RFC822 Name=mick@jones.be, IP Address=110.9.0.0, Other Name:Principal Name=myNameIsBob, URL=https://damienbod.com/"; 

                    Assert.Equal(expected, data);
                    return;
                }
            }

            throw new Exception("no SubjectAlternativeName found");
        }

        public X509Certificate2 CreateSubjectAlternativeNameDetails(
           SubjectAlternativeName subjectAlternativeName,
           CreateCertificates createCertificates)
        {
            var distinguishedName = new DistinguishedName
            {
                CommonName = "root dev",
                Country = "IT",
                Locality = "DD",
                Organisation = "SS",
                OrganisationUnit = "unit",
                StateProvince = "yes"
            };
            var enhancedKeyUsages = new OidCollection {
                OidLookup.ClientAuthentication,
                OidLookup.ServerAuthentication
            };

            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 3,
                Critical = true
            };

            var validityPeriod = new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) };

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign;

            var rootCert = createCertificates.NewECDsaSelfSignedCertificate(
                distinguishedName,
                basicConstraints,
                validityPeriod,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new ECDsaConfiguration());

            return rootCert;
        }
    }
}
