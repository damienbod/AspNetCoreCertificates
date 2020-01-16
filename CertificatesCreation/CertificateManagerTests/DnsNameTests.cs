using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CertificateManagerTests
{
    public class DnsNameTests
    {

        [Fact]
        public void DnsNameValid()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            var rootCaL1 = certManagerService.CreateRootCertificateForClientServerAuth(
                new DistinguishedName { CommonName = "root dev", Country = "IT" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "localhost");

            Assert.Equal("CN=root dev, C=IT", rootCaL1.Subject);
        }

        [Fact]
        public void DnsNameInvalid()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var certManagerService = serviceProvider.GetService<CertificateManagerService>();


            certManagerService.CreateRootCertificateForClientServerAuth(
                new DistinguishedName
                {
                    CommonName = "root dev",
                    Country = "IT",
                    Locality = "DD",
                    Organisation = "SS"

                },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                3, "local  _ host");
        }


        

    }
}
