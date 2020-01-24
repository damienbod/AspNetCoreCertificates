using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreateAngularVueJsDevelopmentCertificates
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddCertificateManager()
                .BuildServiceProvider();

            var _createCertificatesRsa = serviceProvider.GetService<CreateCertificatesRsa>();

            // Create development certificate for localhost
            var rootCert = _createCertificatesRsa
                .CreateDevelopmentCertificate("localhost", 10);

            rootCert.FriendlyName = "localhost development";
        }
    }
}
