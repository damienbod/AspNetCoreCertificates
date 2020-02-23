# X509 Certificates WIKI

## What is a self signed certificate

A self signed certificate is any certificate which is not signed by a certificate authority.

All root certificates are self signed.

## What is a chained certificate

## What is a root certificate

The root certificate is a self signed certificate with constraints on the extensions if required.

If you create your own root certificate, this needs to be added to the Trusted Root Certification Authorities on a Windows PC.

Also to the Firefox trusted certificates.

## What is a intermiedate certificate

## Certificate Types

## CRL Distribution Points

https://www.alvestrand.no/objectid/2.5.29.31.html

https://github.com/dotnet/runtime/blob/f1c7ed882a90104ab7171c95f4767fc15bb50cd6/src/libraries/System.Security.Cryptography.X509Certificates/tests/RevocationTests/CertificateAuthority.cs

https://docs.microsoft.com/en-us/powershell/module/adcsadministration/add-cacrldistributionpoint?view=win10-ps

## Certificate Transparency

https://scotthelme.co.uk/certificate-transparency-an-introduction/

https://tools.ietf.org/html/rfc6962

https://www.certificate-transparency.org/

https://crt.sh/

## OCSP Stapling

https://scotthelme.co.uk/ocsp-stapling-speeding-up-ssl/

https://en.wikipedia.org/wiki/Online_Certificate_Status_Protocol

## Certificate Properties

**Version**: X.509v3

**Serial number**: Unique identifier within a certification authority

**Signature algorithm**: SHA usually

**Issuer**: Distinguished Name
 
**Subject**: Distinguished Name 

**Validity period**

- Valid From
- Valid To  

**Public Key**

**Extensions**

### Distinguished Name:

example:
```
C=CH, O=damienbod, OU=testing, CN=localhost
```

C= Country 

ST= State or province

L= Locality

O= organisation

OU=Organisation Unit

CN= Common name (DNS) 

// CN is REQUIRED

## Certificate Extensions, OID Object Identifiers

https://www.alvestrand.no/objectid/

### C#

- X509BasicConstraintsExtension
- X509KeyUsageExtension OID KU
- X509EnhancedKeyUsageExtension => OID EKU


```
new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth

new Oid("2.5.29.35") // authorityKeyIdentifier

new Oid("2.5.29.19") // - Basic Constraints
```

ref:

https://access.redhat.com/documentation/en-US/Red_Hat_Certificate_System/8.0/html/Admin_Guide/Standard_X.509_v3_Certificate_Extensions.html

#### OID 2.5.29.35 authorityKeyIdentifier
The Authority Key Identifier extension identifies the public key corresponding to the private key used to sign a certificate. This extension is useful when an issuer has multiple signing keys, such as when a CA certificate is renewed.

The extension consists of one or both of the following:
- An explicit key identifier, set in the keyIdentifier field
- An issuer, set in the authorityCertIssuer field, and serial number, set in the authorityCertSerialNumber field, identifying a certificate

If the keyIdentifier field exists, it is used to select the certificate with a matching subjectKeyIdentifier extension. If the authorityCertIssuer and authorityCertSerialNumber fields are present, then they are used to identify the correct certificate by issuer and serialNumber.

If this extension is not present, then the issuer name alone is used to identify the issuer certificate.
PKIX Part 1 requires this extension for all certificates except self-signed root CA certificates. Where a key identifier has not been established, PKIX recommends that the authorityCertIssuer and authorityCertSerialNumber fields be specified. These fields permit construction of a complete certificate chain by matching the SubjectName and CertificateSerialNumber fields in the issuer's certificate against the authortiyCertIssuer and authorityCertSerialNumber in the Authority Key Identifier extension of the subject certificate.
OID

#### OID 2.5.29.19 basicConstraints
This extension is used during the certificate chain verification process to identify CA certificates and to apply certificate chain path length constraints. The cA component should be set to true for all CA certificates. PKIX recommends that this extension should not appear in end-entity certificates.

If the pathLenConstraint component is present, its value must be greater than the number of CA certificates that have been processed so far, starting with the end-entity certificate and moving up the chain. If pathLenConstraint is omitted, then all of the higher level CA certificates in the chain must not include this component when the extension is present.

```
//
// Summary:
//     Defines how the certificate key can be used. If this value is not defined, the
//     key can be used for any purpose.
[Flags]
public enum X509KeyUsageFlags
{
	//
	// Summary:
	//     No key usage parameters.
	None = 0,
	//
	// Summary:
	//     The key can be used for encryption only.
	EncipherOnly = 1,
	//
	// Summary:
	//     The key can be used to sign a certificate revocation list (CRL).
	CrlSign = 2,
	//
	// Summary:
	//     The key can be used to sign certificates.
	KeyCertSign = 4,
	//
	// Summary:
	//     The key can be used to determine key agreement, such as a key created using the
	//     Diffie-Hellman key agreement algorithm.
	KeyAgreement = 8,
	//
	// Summary:
	//     The key can be used for data encryption.
	DataEncipherment = 16,
	//
	// Summary:
	//     The key can be used for key encryption.
	KeyEncipherment = 32,
	//
	// Summary:
	//     The key can be used for authentication.
	NonRepudiation = 64,
	//
	// Summary:
	//     The key can be used as a digital signature.
	DigitalSignature = 128,
	//
	// Summary:
	//     The key can be used for decryption only.
	DecipherOnly = 32768
}
```

### other definitions 

**SAN**: Subject Alternative Name

**ECDSA**: Elliptic Curve Digital Signature Algorithm (ECDSA)

## Certificate Types

## Certificate File types

### .pfx

X509ContentType.Pkcs12

### .cer

Binary

### .pem

It is the most common format that Certificate Authorities issue certificates in. It contains the ‘—–BEGIN CERTIFICATE—–” and “—–END CERTIFICATE—–” statements.
The files are Base64 encoded ACII files.

https://gist.github.com/ChrisTowles/f8a5358a29aebcc23316605dd869e839


https://serverfault.com/questions/9708/what-is-a-pem-file-and-how-does-it-differ-from-other-openssl-generated-key-file

From the above link:

.csr - This is a Certificate Signing Request. Some applications can generate these for submission to certificate-authorities. The actual format is PKCS10 which is defined in RFC 2986. It includes some/all of the key details of the requested certificate such as subject, organization, state, whatnot, as well as the public key of the certificate to get signed. These get signed by the CA and a certificate is returned. The returned certificate is the public certificate (which includes the public key but not the private key), which itself can be in a couple of formats.

.pem - Defined in RFCs 1421 through 1424, this is a container format that may include just the public certificate (such as with Apache installs, and CA certificate files /etc/ssl/certs), or may include an entire certificate chain including public key, private key, and root certificates. Confusingly, it may also encode a CSR (e.g. as used here) as the PKCS10 format can be translated into PEM. The name is from Privacy Enhanced Mail (PEM), a failed method for secure email but the container format it used lives on, and is a base64 translation of the x509 ASN.1 keys.

.key - This is a PEM formatted file containing just the private-key of a specific certificate and is merely a conventional name and not a standardized one. In Apache installs, this frequently resides in /etc/ssl/private. The rights on these files are very important, and some programs will refuse to load these certificates if they are set wrong.

.pkcs12 .pfx .p12 - Originally defined by RSA in the Public-Key Cryptography Standards (abbreviated PKCS), the "12" variant was originally enhanced by Microsoft, and later submitted as RFC 7292. This is a passworded container format that contains both public and private certificate pairs. Unlike .pem files, this container is fully encrypted. Openssl can turn this into a .pem file with both public and private keys: openssl pkcs12 -in file-to-convert.p12 -out converted-file.pem -nodes

A few other formats that show up from time to time:

.der - A way to encode ASN.1 syntax in binary, a .pem file is just a Base64 encoded .der file. OpenSSL can convert these to .pem (openssl x509 -inform der -in to-convert.der -out converted.pem). Windows sees these as Certificate files. By default, Windows will export certificates as .DER formatted files with a different extension. Like...

.cert .cer .crt - A .pem (or rarely .der) formatted file with a different extension, one that is recognized by Windows Explorer as a certificate, which .pem is not.

.p7b .keystore - Defined in RFC 2315 as PKCS number 7, this is a format used by Windows for certificate interchange. Java understands these natively, and often uses .keystore as an extension instead. Unlike .pem style certificates, this format has a defined way to include certification-path certificates.

.crl - A certificate revocation list. Certificate Authorities produce these as a way to de-authorize certificates before expiration. You can sometimes download them from CA websites.

In summary, there are four different ways to present certificates and their components:

PEM - Governed by RFCs, its used preferentially by open-source software. It can have a variety of extensions (.pem, .key, .cer, .cert, more)

PKCS7 - An open standard used by Java and supported by Windows. Does not contain private key material.

PKCS12 - A Microsoft private standard that was later defined in an RFC that provides enhanced security versus the plain-text PEM format. This can contain private key material. Its used preferentially by Windows systems, and can be freely converted to PEM format through use of openssl.

DER - The parent format of PEM. It's useful to think of it as a binary version of the base64-encoded PEM file. Not routinely used very much outside of Windows.

## Public Key Infrastructure - PKI

https://gist.github.com/Soarez/9688998

https://github.com/PKISharp/ACMESharpCore

## Ways of creating certificates

- powershell
- openssl
- C# code

# Links

https://tools.ietf.org/html/rfc5280

https://crt.sh/

https://cabforum.org/baseline-requirements-documents/

https://letsencrypt.org/

https://github.com/rwatjen/AzureIoTDPSCertificates

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-x509ca-overview

https://github.com/Azure/azure-iot-sdk-c/blob/master/tools/CACertificates/CACertificateOverview.md

http://oid-info.com/

https://www.alvestrand.no/objectid/

https://azure.microsoft.com/sv-se/blog/installing-certificates-into-iot-devices/


https://github.com/damienbod/AspNetCoreCertificateAuth

https://github.com/damienbod/Secure_gRpc

https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth

https://docs.microsoft.com/en-us/archive/blogs/kaushal/various-ssltls-certificate-file-typesextensions

https://access.redhat.com/documentation/en-US/Red_Hat_Certificate_System/8.0/html/Admin_Guide/Standard_X.509_v3_Certificate_Extensions.html

https://github.com/nomailme/TestAuthority

### mtls

https://tools.ietf.org/html/draft-ietf-oauth-mtls-14

https://identityserver4.readthedocs.io/en/latest/topics/mtls.html

https://tools.ietf.org/html/rfc7519

https://openid.net/specs/openid-connect-core-1_0.html

https://openid.net/specs/draft-jones-json-web-token-07.html 

https://docs.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth