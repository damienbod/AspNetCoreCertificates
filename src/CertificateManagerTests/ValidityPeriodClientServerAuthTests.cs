using CertificateManager;
using CertificateManager.Models;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace CertificateManagerTests
{
    public class ValidityPeriodClientServerAuthTests
    {
        private (X509Certificate2 root, X509Certificate2 intermediate, X509Certificate2 server, X509Certificate2 client) SetupCerts()
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

            var rootCaL1 = createClientServerAuthCerts.NewRootCertificate(
                new DistinguishedName { CommonName = "root dev", Country = "IT" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow.AddMonths(3), ValidTo = DateTime.UtcNow.AddMonths(6) },
                3, "localhost");
            rootCaL1.FriendlyName = "developement root L1 certificate";

            // Intermediate L2 chained from root L1
            var intermediateCaL2 = createClientServerAuthCerts.NewIntermediateChainedCertificate(
                new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow.AddMonths(2), ValidTo = DateTime.UtcNow.AddYears(1) },
                2, "localhost", rootCaL1);
            intermediateCaL2.FriendlyName = "developement Intermediate L2 certificate";

            // Server, Client L3 chained from Intermediate L2
            var serverL3 = createClientServerAuthCerts.NewServerChainedCertificate(
                new DistinguishedName { CommonName = "server", Country = "DE" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);

            var clientL3 = createClientServerAuthCerts.NewClientChainedCertificate(
                new DistinguishedName { CommonName = "client", Country = "IE" },
                new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
                "localhost", intermediateCaL2);
            serverL3.FriendlyName = "developement server L3 certificate";
            clientL3.FriendlyName = "developement client L3 certificate";

            return (rootCaL1, intermediateCaL2, serverL3, clientL3);
        }

        [Fact]
        public void ClientServerCertCorrectValidityPeriod()
        {
            // ValidityPeriod definitions
            // client/server t <= intermediate <= root <= intermediate <= client/server
            var (root, intermediate, server, client) = SetupCerts();

            Assert.Equal(root.NotBefore, intermediate.NotBefore);
            Assert.Equal(root.NotAfter, intermediate.NotAfter);

            Assert.Equal(intermediate.NotBefore, client.NotBefore);
            Assert.Equal(intermediate.NotAfter, client.NotAfter);

            Assert.Equal(intermediate.NotBefore, server.NotBefore);
            Assert.Equal(intermediate.NotAfter, server.NotAfter);

        }

        [Fact]
        public void ValidateChainedValidityPeriodNotActive()
        {
            // certs are not active till the future
            var (root, intermediate, server, client) = SetupCerts();

            var x509ChainPolicy = BuildChainUtil.BuildChainPolicyChained(
                root, intermediate, server, client,
                X509RevocationFlag.ExcludeRoot,
                X509RevocationMode.NoCheck,
                true, true);

            var chain = new X509Chain
            {
                ChainPolicy = x509ChainPolicy
            };

            var certificateIsValid = chain.Build(client);
            Assert.False(certificateIsValid);

            if (!certificateIsValid)
            {
                var chainErrors = new List<X509ChainStatusFlags>();
                foreach (var validationFailure in chain.ChainStatus)
                {
                    chainErrors.Add(validationFailure.Status);
                }
                Assert.True(chainErrors.Contains(X509ChainStatusFlags.NotTimeValid), "expect NotValidForUsage");
            }
        }
    }
}
