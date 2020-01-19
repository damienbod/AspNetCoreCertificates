using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace CertificateManagerTests
{
    public class DistinguishedNameTests
    {

        [Fact]
        public void DnCompleteValid()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();


            var rootCaL1 = createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName { 
                    CommonName = "root dev", 
                    Country = "IT", 
                    Locality = "DD", 
                    Organisation="SS", 
                    OrganisationUnit="unit",
                    StateProvince= "yes"
                },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");

            Assert.Equal("CN=root dev, C=IT, O=SS, OU=unit, L=DD, S=yes", rootCaL1.Subject);
            Assert.Equal("CN=root dev, C=IT, O=SS, OU=unit, L=DD, S=yes", rootCaL1.Issuer);
        }

        [Fact]
        public void DnHalfCompleteValid()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();


            var rootCaL1 = createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName
                {
                    CommonName = "root dev",
                    Country = "IT",
                    Locality = "DD",
                    Organisation = "SS"

                },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");

            Assert.Equal("CN=root dev, C=IT, O=SS, L=DD", rootCaL1.Subject);
            Assert.Equal("CN=root dev, C=IT, O=SS, L=DD", rootCaL1.Issuer);
        }

        [Fact]
        public void DnMissingCountry()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName
                {
                    CommonName = "root dev",
                    Locality = "DD",
                    Organisation = "SS",
                    OrganisationUnit = "unit",
                    StateProvince = "yes"
                },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            });
        }

        [Fact]
        public void DnMissingCommonName()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName
                {
                    Country = "IT",
                    Locality = "DD",
                    Organisation = "SS",
                    OrganisationUnit = "unit",
                    StateProvince = "yes"
                },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            });
        }

        [Fact]
        public void DnNull()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var exception = Assert.Throws<NullReferenceException>(() =>
            {
                createClientServerAuthCerts.NewRootCertificate(
                null,
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
            });
        }
    }
}
