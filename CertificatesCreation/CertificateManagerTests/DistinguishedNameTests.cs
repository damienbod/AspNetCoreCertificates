using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            var rootCaL1 = certManagerService.CreateRootCertificateForClientServerAuth(
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

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            var rootCaL1 = certManagerService.CreateRootCertificateForClientServerAuth(
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

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            certManagerService.CreateRootCertificateForClientServerAuth(
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

        }

        [Fact]
        public void DnMissingCommonName()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            certManagerService.CreateRootCertificateForClientServerAuth(
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
        }

        [Fact]
        public void DnNull()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            certManagerService.CreateRootCertificateForClientServerAuth(
                null,
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");
        }
    }
}
