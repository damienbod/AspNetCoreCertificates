using System.Text;

namespace CertificateManager
{
    public class PemParser
    {
        //RSA Public Key
        //-----BEGIN RSA PUBLIC KEY-----
        //-----END RSA PUBLIC KEY-----
        
        //Encrypted Private Key
        //-----BEGIN RSA PRIVATE KEY-----
        //Proc-Type: 4,ENCRYPTED
        //-----END RSA PRIVATE KEY-----

        //CRL
        //-----BEGIN X509 CRL-----
        //-----END X509 CRL-----

        //CRT
        //-----BEGIN CERTIFICATE-----
        //-----END CERTIFICATE-----
        public string ProcessCrt(string pemPart)
        {
            var sb = new StringBuilder(pemPart);
            sb.Replace(PemTypes.BEGIN_CERTIFICATE, string.Empty);
            sb.Replace(PemTypes.END_CERTIFICATE, string.Empty);
            sb.Replace("\r\n", string.Empty);
            sb.Replace("\n", string.Empty);

            return sb.ToString();
        }
        //CSR
        //-----BEGIN CERTIFICATE REQUEST-----
        //-----END CERTIFICATE REQUEST-----

        //NEW CSR
        //-----BEGIN NEW CERTIFICATE REQUEST-----
        //-----END NEW CERTIFICATE REQUEST-----

        //PEM
        //-----END RSA PRIVATE KEY-----
        //-----BEGIN RSA PRIVATE KEY-----

        //PKCS7
        //-----BEGIN PKCS7-----
        //-----END PKCS7-----

        //PRIVATE KEY
        //-----BEGIN PRIVATE KEY-----
        //-----END PRIVATE KEY-----

        //DSA KEY
        //-----BEGIN DSA PRIVATE KEY-----
        //-----END DSA PRIVATE KEY-----

        //Elliptic Curve
        //-----BEGIN EC PRIVATE KEY-----
        //-----BEGIN EC PRIVATE KEY-----

        //PGP Private Key
        //-----BEGIN PGP PRIVATE KEY BLOCK-----
        //-----END PGP PRIVATE KEY BLOCK-----

        //PGP Public Key
        //-----BEGIN PGP PUBLIC KEY BLOCK-----
        //-----END PGP PUBLIC KEY BLOCK-----

    }
}
