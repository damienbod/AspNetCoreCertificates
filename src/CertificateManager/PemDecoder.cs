using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace CertificateManager
{
    /// <summary>
    /// src:  Mathias Raacke
    /// https://github.com/oocx/ReadX509CertificateFromPem
    /// </summary>
    public static class PemDecoder
    {
        public static byte[] DecodeSection(string data, string type)
        {
            var lines = data.Replace("\r", "").Split("\n");
            bool inSection = false;
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (!inSection)
                {
                    if (line == $"-----BEGIN {type}-----")
                    {
                        inSection = true;
                    }
                }
                else
                {
                    if (line == $"-----END {type}-----")
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(line);
                    }

                }
            }
            return Convert.FromBase64String(sb.ToString());
        }

        public static string GetBegin(string type)
        {
            return $"-----BEGIN {type}-----";
        }

        public static string GetEnd(string type)
        {
            return $"-----END {type}-----";
        }

        public static AsymmetricAlgorithm LoadPrivateKey(string privateKeyPem)
        {
            var keyType = DetectKeyType(privateKeyPem);
            var privateKeyBytes = PemDecoder.DecodeSection(privateKeyPem, keyType);
            var privateKey = GetPrivateKey(keyType, new ReadOnlyMemory<byte>(privateKeyBytes));
            return privateKey;
        }

        public static string DetectKeyType(string pem)
        {
            var keyTypeRegEx = new Regex("-----BEGIN ([A-Z\\s]*)");
            var matches = keyTypeRegEx.Matches(pem);

            if (matches.Count == 0) { throw new ArgumentException("No private key found"); }
            for (int mc = 0; mc < matches.Count; mc++)
            {
                for (int g = 0; g < matches[mc].Groups.Count; g++)
                {
                    if (PemTypes.KnownTypes.Contains(matches[mc].Groups[g].Value))
                    {
                        return matches[mc].Groups[g].Value;
                    }
                }
            }
            throw new ArgumentException("No supported key detected");
        }

        public static AsymmetricAlgorithm GetPrivateKey(string keyType, ReadOnlyMemory<byte> privateKeyBytes)
        {
            AsymmetricAlgorithm ECKey(ReadOnlySpan<byte> bytes)
            {
                var ecdh = ECDiffieHellman.Create();
                ecdh.ImportECPrivateKey(bytes, out _);
                return ecdh;
            }

            AsymmetricAlgorithm ECKeyPkcs8(ReadOnlySpan<byte> bytes)
            {
                var ecdh = ECDiffieHellman.Create();
                ecdh.ImportPkcs8PrivateKey(bytes, out _);
                return ecdh;
            }

            AsymmetricAlgorithm RSAKey(ReadOnlySpan<byte> bytes)
            {
                var rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(bytes, out _);
                return rsa;
            }

            switch (keyType)
            {

                case PemTypes.EC_PRIVATE_KEY: return ECKey(privateKeyBytes.Span);
                case PemTypes.RSA_PRIVATE_KEY: return RSAKey(privateKeyBytes.Span);
                case PemTypes.PRIVATE_KEY:
                    var key = Pkcs8PrivateKeyInfo.Decode(privateKeyBytes, out _);
                    if (key.AlgorithmId.FriendlyName == "RSA")
                    {
                        return RSAKey(key.PrivateKeyBytes.Span);
                    }

                    if (key.AlgorithmId.FriendlyName == "ECC")
                    {
                        return ECKeyPkcs8(privateKeyBytes.Span);
                    }
                    throw new ArgumentException($"Unsupported private key type {key.AlgorithmId.FriendlyName}");
                default:
                    throw new ArgumentException($"Unsupported key type: {keyType}.");
            }
        }

        public static X509Certificate2 CreateCertificateWithPrivateKey(
            X509Certificate2 certificate, 
            AsymmetricAlgorithm privateKey, 
            string password = null)
        {
            var builder = new Pkcs12Builder();
            var contents = new Pkcs12SafeContents();
            contents.AddCertificate(certificate);
            contents.AddKeyUnencrypted(privateKey);
            builder.AddSafeContentsUnencrypted(contents);

            // OpenSSL requires the file to have a mac, without mac this will run on Windows but not on Linux
            builder.SealWithMac(password, HashAlgorithmName.SHA256, 1);
            var pkcs12bytes = builder.Encode();

            if (string.IsNullOrEmpty(password))
            {
                var certificateOut = new X509Certificate2(pkcs12bytes);
                return certificateOut;
            }
            else
            {
                var certificateOut = new X509Certificate2(pkcs12bytes, password);
                return certificateOut;
            }
        }
    }
}
