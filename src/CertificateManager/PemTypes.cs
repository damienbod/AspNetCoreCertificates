using System;
using System.Collections.Generic;
using System.Text;

namespace CertificateManager
{
    public static class PemTypes
    {
        /// <summary>
        /// RSA Public Key
        /// </summary>
        public static string BEGIN_RSA_PUBLIC_KEY = "-----BEGIN RSA PUBLIC KEY-----";
        public static string END_RSA_PUBLIC_KEY = "-----END RSA PUBLIC KEY-----";

        /// <summary>
        /// Encrypted Private Key / PEM
        /// </summary>
        public static string BEGIN_RSA_PRIVATE_KEY = "-----BEGIN RSA PRIVATE KEY-----";
        public static string END_RSA_PRIVATE_KEY = "-----END RSA PRIVATE KEY-----";

        /// <summary>
        /// CRL
        /// </summary>
        public static string BEGIN_X509_CRL = "-----BEGIN X509 CRL-----";
        public static string END_X509_CRL = "-----END X509 CRL-----";

        /// <summary>
        /// CRT
        /// </summary>
        public static string BEGIN_CERTIFICATE = "-----BEGIN CERTIFICATE-----";
        public static string END_CERTIFICATE = "-----END CERTIFICATE-----";

        /// <summary>
        /// CSR
        /// </summary>
        public static string BEGIN_CERTIFICATE_REQUEST = "-----BEGIN CERTIFICATE REQUEST-----";
        public static string END_CERTIFICATE_REQUEST = "-----END CERTIFICATE REQUEST-----";

        /// <summary>
        /// NEW CSR
        /// </summary>
        public static string BEGIN_NEW_CERTIFICATE_REQUEST = "-----BEGIN NEW CERTIFICATE REQUEST-----";
        public static string END_NEW_CERTIFICATE_REQUEST = "-----END NEW CERTIFICATE REQUEST-----";

        /// <summary>
        /// PKCS7
        /// </summary>
        public static string BEGIN_PKCS7 = "-----BEGIN PKCS7-----";
        public static string END_PKCS7 = "-----END PKCS7-----";


        /// <summary>
        /// PRIVATE KEY
        /// </summary>
        public static string BEGIN_PRIVATE_KEY = "-----BEGIN PRIVATE KEY-----";
        public static string END_PRIVATE_KEY = "-----END PRIVATE KEY-----";

        /// <summary>
        /// DSA KEY
        /// </summary>
        public static string BEGIN_DSA_PRIVATE_KEY = "-----BEGIN DSA PRIVATE KEY-----";
        public static string END_DSA_PRIVATE_KEY = "-----END DSA PRIVATE KEY-----";

        /// <summary>
        /// Elliptic Curve
        /// </summary>
        public static string BEGIN_EC_PRIVATE_KEY = "-----BEGIN EC PRIVATE KEY-----";
        public static string END_EC_PRIVATE_KEY = "-----END EC PRIVATE KEY-----";

        /// <summary>
        /// PGP Private Key
        /// </summary>
        public static string BEGIN_PGP_PRIVATE_KEY_BLOCK = "-----BEGIN PGP PRIVATE KEY BLOCK-----";
        public static string END_PGP_PRIVATE_KEY_BLOCK = "-----END PGP PRIVATE KEY BLOCK-----";

        /// <summary>
        /// PGP Public Key
        /// </summary>
        public static string BEGIN_PGP_PUBLIC_KEY_BLOCK = "-----BEGIN PGP PUBLIC KEY BLOCK-----";
        public static string END_PGP_PUBLIC_KEY_BLOCK = "-----END PGP PUBLIC KEY BLOCK-----";
    }
}
