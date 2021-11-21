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
        public const string RSA_PUBLIC_KEY = "RSA PUBLIC KEY";

        /// <summary>
        /// Encrypted Private Key / PEM
        /// PKCS#1
        /// </summary>
        public const string RSA_PRIVATE_KEY = "RSA PRIVATE KEY";

        /// <summary>
        /// CRL
        /// </summary>
        public const string X509_CRL = "X509 CRL";

        /// <summary>
        /// CRT
        /// </summary>
        public const string CERTIFICATE = "CERTIFICATE";

        /// <summary>
        /// CSR
        /// </summary>
        public const string CERTIFICATE_REQUEST = "CERTIFICATE REQUEST";

        /// <summary>
        /// NEW CSR
        /// </summary>
        public const string NEW_CERTIFICATE_REQUEST = "NEW CERTIFICATE REQUEST";

        /// <summary>
        /// PKCS7
        /// </summary>
        public const string PKCS7 = "PKCS7";


        /// <summary>
        /// PRIVATE KEY
        /// PKCS#8 RFC5208 RFC5958
        /// </summary>
        public const string PRIVATE_KEY = "PRIVATE KEY";

        /// <summary>
        /// DSA KEY
        /// </summary>
        public const string DSA_PRIVATE_KEY = "DSA PRIVATE KEY";

        /// <summary>
        /// Elliptic Curve
        /// RFC5915
        /// </summary>
        public const string EC_PRIVATE_KEY = "EC PRIVATE KEY";

        /// <summary>
        /// PGP Private Key
        /// </summary>
        public const string PGP_PRIVATE_KEY_BLOCK = "PGP PRIVATE KEY BLOCK";

        /// <summary>
        /// PGP Public Key
        /// </summary>
        public const string PGP_PUBLIC_KEY_BLOCK = "PGP PUBLIC KEY BLOCK";

        /// <summary>
        /// ENCRYPTED_PRIVATE_KEY
        /// PKCS#8 RFC5208
        /// </summary>
        public const string ENCRYPTED_PRIVATE_KEY = "ENCRYPTED PRIVATE KEY";

        public static readonly string[] KnownTypes = new[] { RSA_PRIVATE_KEY, PRIVATE_KEY, ENCRYPTED_PRIVATE_KEY, EC_PRIVATE_KEY };

    }
}
