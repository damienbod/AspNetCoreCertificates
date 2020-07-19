using System.Security.Cryptography;

namespace CertificateManager
{
    public static class OidLookup
    {
        /// <summary>
        /// 1.3.6.1.4.1.311.76.6.1
        /// </summary>
        public static Oid WindowsUpdate { get; } = new Oid("1.3.6.1.4.1.311.76.6.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.25
        /// </summary>
        public static Oid WindowsThirdPartyApplicationComponent { get; } = new Oid("1.3.6.1.4.1.311.10.3.25");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.6
        /// </summary>
        public static Oid WindowsSystemComponentVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.6");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.23
        /// </summary>
        public static Oid WindowsTCBComponent { get; } = new Oid("1.3.6.1.4.1.311.10.3.23");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.26
        /// </summary>
        public static Oid WindowsSoftwareExtensionVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.26");
        /// <summary>
        /// 1.3.6.1.4.1.311.76.3.1
        /// </summary>
        public static Oid WindowsStore { get; } = new Oid("1.3.6.1.4.1.311.76.3.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.20
        /// </summary>
        public static Oid WindowsKitsComponent { get; } = new Oid("1.3.6.1.4.1.311.10.3.20");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.5.1
        /// </summary>
        public static Oid WindowsHardwareDriverAttestedVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.5.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.5
        /// </summary>
        public static Oid WindowsHardwareDriverVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.5");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.39
        /// </summary>
        public static Oid WindowsHardwareDriverExtendedVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.39");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.21
        /// </summary>
        public static Oid WindowsRTVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.21");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.11
        /// </summary>
        public static Oid KeyRecovery { get; } = new Oid("1.3.6.1.4.1.311.10.3.11");
        /// <summary>
        /// 1.3.6.1.4.1.311.21.6
        /// </summary>
        public static Oid KeyRecoveryAgent { get; } = new Oid("1.3.6.1.4.1.311.21.6");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.6.1
        /// </summary>
        public static Oid KeyPackLicenses { get; } = new Oid("1.3.6.1.4.1.311.10.6.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.61.4.1
        /// </summary>
        public static Oid EarlyLaunchAntimalwareDriver { get; } = new Oid("1.3.6.1.4.1.311.61.4.1");
        /// <summary
        /// 1.3.6.1.4.1.311.61.1.1
        /// </summary>
        public static Oid KernelModeCodeSigning { get; } = new Oid("1.3.6.1.4.1.311.61.1.1");
        /// <summary>
        /// 2.23.133.8.3
        /// </summary>
        public static Oid AttestationIdentityKeyCertificate { get; } = new Oid("2.23.133.8.3");
        /// <summary>
        /// 1.3.6.1.4.1.311.20.2.2
        /// </summary>
        public static Oid SmartCardLogon { get; } = new Oid("1.3.6.1.4.1.311.20.2.2");
        /// <summary>
        /// 1.3.6.1.5.2.3.5
        /// </summary>
        public static Oid KDCAuthentication { get; } = new Oid("1.3.6.1.5.2.3.5");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.8
        /// </summary>
        public static Oid EmbeddedWindowsSystemComponentVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.8");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.6
        /// </summary>
        public static Oid IPsecuritytunneltermination { get; } = new Oid("1.3.6.1.5.5.7.3.6");
        /// <summary>
        /// 1.3.6.1.5.5.8.2.2
        /// </summary>
        public static Oid IPsecurityIKEintermediate { get; } = new Oid("1.3.6.1.5.5.8.2.2");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.7
        /// </summary>
        public static Oid IPsecurityuser { get; } = new Oid("1.3.6.1.5.5.7.3.7");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.6.2
        /// </summary>
        public static Oid LicenseServerVerification { get; } = new Oid("1.3.6.1.4.1.311.10.6.2");
        /// <summary>
        /// 1.3.6.1.4.1.311.76.5.1
        /// </summary>
        public static Oid DynamicCodeGenerator { get; } = new Oid("1.3.6.1.4.1.311.76.5.1");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.8
        /// </summary>
        public static Oid TimeStamping { get; } = new Oid("1.3.6.1.5.5.7.3.8");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.4.1
        /// </summary>
        public static Oid FileRecovery { get; } = new Oid("1.3.6.1.4.1.311.10.3.4.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.2.6.1
        /// </summary>
        public static Oid SpcRelaxedPEMarkerCheck { get; } = new Oid("1.3.6.1.4.1.311.2.6.1");
        /// <summary>
        /// 2.23.133.8.1
        /// </summary>
        public static Oid EndorsementKeyCertificate { get; } = new Oid("2.23.133.8.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.2.6.2
        /// </summary>
        public static Oid SpcEncryptedDigestRetryCount { get; } = new Oid("1.3.6.1.4.1.311.2.6.2");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.4
        /// </summary>
        public static Oid EncryptingFileSystem { get; } = new Oid("1.3.6.1.4.1.311.10.3.4");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.1
        /// </summary>
        public static Oid ServerAuthentication { get; } = new Oid("1.3.6.1.5.5.7.3.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.61.5.1
        /// </summary>
        public static Oid HALExtension { get; } = new Oid("1.3.6.1.4.1.311.61.5.1");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.4
        /// </summary>
        public static Oid SecureEmail { get; } = new Oid("1.3.6.1.5.5.7.3.4");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.5
        /// </summary>
        public static Oid IPsecurityendsystem { get; } = new Oid("1.3.6.1.5.5.7.3.5");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.9
        /// </summary>
        public static Oid RootListSigner { get; } = new Oid("1.3.6.1.4.1.311.10.3.9");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.30
        /// </summary>
        public static Oid DisallowedList { get; } = new Oid("1.3.6.1.4.1.311.10.3.30");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.19
        /// </summary>
        public static Oid RevokedListSigner { get; } = new Oid("1.3.6.1.4.1.311.10.3.19");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.10
        /// </summary>
        public static Oid QualifiedSubordination { get; } = new Oid("1.3.6.1.4.1.311.10.3.10");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.12
        /// </summary>
        public static Oid DocumentSigning { get; } = new Oid("1.3.6.1.4.1.311.10.3.12");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.24
        /// </summary>
        public static Oid ProtectedProcessVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.24");
        /// <summary>
        /// 1.3.6.1.4.1.311.80.1
        /// </summary>
        public static Oid DocumentEncryption { get; } = new Oid("1.3.6.1.4.1.311.80.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.22
        /// </summary>
        public static Oid ProtectedProcessLightVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.22");
        /// <summary>
        /// 1.3.6.1.4.1.311.21.19
        /// </summary>
        public static Oid DirectoryServiceEmailReplication { get; } = new Oid("1.3.6.1.4.1.311.21.19");
        /// <summary>
        /// 1.3.6.1.4.1.311.21.5
        /// </summary>
        public static Oid PrivateKeyArchival { get; } = new Oid("1.3.6.1.4.1.311.21.5");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.5.1
        /// </summary>
        public static Oid DigitalRights { get; } = new Oid("1.3.6.1.4.1.311.10.5.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.27
        /// </summary>
        public static Oid PreviewBuildSigning { get; } = new Oid("1.3.6.1.4.1.311.10.3.27");
        /// <summary>
        /// 1.3.6.1.4.1.311.20.2.1
        /// </summary>
        public static Oid CertificateRequestAgent { get; } = new Oid("1.3.6.1.4.1.311.20.2.1");
        /// <summary>
        /// 2.23.133.8.2
        /// </summary>
        public static Oid PlatformCertificate { get; } = new Oid("2.23.133.8.2");
        /// <summary>
        /// 1.3.6.1.4.1.311.20.1
        /// </summary>
        public static Oid CTLUsage { get; } = new Oid("1.3.6.1.4.1.311.20.1");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.9
        /// </summary>
        public static Oid OCSPSigning { get; } = new Oid("1.3.6.1.5.5.7.3.9");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.3
        /// </summary>
        public static Oid CodeSigning { get; } = new Oid("1.3.6.1.5.5.7.3.3");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.1
        /// </summary>
        public static Oid MicrosoftTrustListSigning { get; } = new Oid("1.3.6.1.4.1.311.10.3.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.2
        /// </summary>
        public static Oid MicrosoftTimeStamping { get; } = new Oid("1.3.6.1.4.1.311.10.3.2");
        /// <summary>
        /// 1.3.6.1.4.1.311.76.8.1
        /// </summary>
        public static Oid MicrosoftPublisher { get; } = new Oid("1.3.6.1.4.1.311.76.8.1");
        /// <summary>
        /// 1.3.6.1.5.5.7.3.2
        /// </summary>
        public static Oid ClientAuthentication { get; } = new Oid("1.3.6.1.5.5.7.3.2");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.13
        /// </summary>
        public static Oid LifetimeSigning { get; } = new Oid("1.3.6.1.4.1.311.10.3.13");
        /// <summary>
        /// 2.5.29.37.0
        /// </summary>
        public static Oid AnyPurpose { get; } = new Oid("2.5.29.37.0");
        /// <summary>
        /// 1.3.6.1.4.1.311.64.1.1
        /// </summary>
        public static Oid DomainNameSystemServerTrust { get; } = new Oid("1.3.6.1.4.1.311.64.1.1");
        /// <summary>
        /// 1.3.6.1.4.1.311.10.3.7
        /// </summary>
        public static Oid OEMWindowsSystemComponentVerification { get; } = new Oid("1.3.6.1.4.1.311.10.3.7");
        /// <summary>
        /// Create New Instance of Custom Oid
        /// </summary>
        public static Oid NewOid(string oid)
        {
            return new Oid(oid);
        }
    }
}