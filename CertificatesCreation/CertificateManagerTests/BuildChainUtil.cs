using Microsoft.AspNetCore.Authentication.Certificate;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertificateManagerTests
{
    public static class BuildChainUtil
    {
        private static readonly Oid ServerCertificateOid = new Oid("1.3.6.1.5.5.7.3.1");
        private static readonly Oid ClientCertificateOid = new Oid("1.3.6.1.5.5.7.3.2");

        public static X509ChainPolicy BuildChainPolicySelfSigned(
            X509Certificate2 certificate,
            bool ValidateCertificateUse,
            bool ValidateValidityPeriod)
        {
            // Turn off chain validation, because we have a self signed certificate.
            var revocationFlag = X509RevocationFlag.EntireChain;
            var revocationMode = X509RevocationMode.NoCheck;
            var chainPolicy = new X509ChainPolicy
            {
                RevocationFlag = revocationFlag,
                RevocationMode = revocationMode,
            };

            if (ValidateCertificateUse)
            {
                chainPolicy.ApplicationPolicy.Add(ClientCertificateOid);
            }

            chainPolicy.VerificationFlags |= X509VerificationFlags.AllowUnknownCertificateAuthority;
            chainPolicy.VerificationFlags |= X509VerificationFlags.IgnoreEndRevocationUnknown;
            chainPolicy.ExtraStore.Add(certificate);

            if (!ValidateValidityPeriod)
            {
                chainPolicy.VerificationFlags |= X509VerificationFlags.IgnoreNotTimeValid;
            }

            return chainPolicy;
        }

        public static X509ChainPolicy BuildChainPolicyChained(
            X509Certificate2 root, X509Certificate2 intermediate,
            X509Certificate2 server, X509Certificate2 client,
            X509RevocationFlag revocationFlag,
            X509RevocationMode revocationMode,
            bool ValidateCertificateUse,
            bool ValidateValidityPeriod)
        {
            var chainPolicy = new X509ChainPolicy
            {
                RevocationFlag = revocationFlag,
                RevocationMode = revocationMode,
            };

            if (ValidateCertificateUse)
            {
                chainPolicy.ApplicationPolicy.Add(ClientCertificateOid);
            }

            // This is NOT the default !!!
            // Only set this to validate the other parts of the chained flow
            chainPolicy.VerificationFlags |= X509VerificationFlags.AllowUnknownCertificateAuthority;

            chainPolicy.ExtraStore.Add(root);
            chainPolicy.ExtraStore.Add(intermediate);
            chainPolicy.ExtraStore.Add(server);
            chainPolicy.ExtraStore.Add(client);

            if (!ValidateValidityPeriod)
            {
                chainPolicy.VerificationFlags |= X509VerificationFlags.IgnoreNotTimeValid;
            }

            return chainPolicy;
        }
    }
}
