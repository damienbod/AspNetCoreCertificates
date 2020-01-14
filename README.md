# ASP.NET Core Certificates



## What is a self signed certificate

## What is a chained certificate

## What is a root certificate

The root certificate is a self signed certificate with constraints on the extensions if required.

If you create your own root certificate, this needs to be added to the Trusted Root Certification Authorities on a Windows PC.

Also to the Firefox trusted certificates.

## What is a intermiedate certificate

## Certificate Properties

Version: X.509v3

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

C= Country REQUIRED
ST= State or province
L= Locality
O= organisation
OU=Organisation Unit
CN= Common name (DNS)  REQUIRED
E=email

## Certificate Extensions, OID

### C#

- X509BasicConstraintsExtension
- X509KeyUsageExtension
- X509EnhancedKeyUsageExtension => OID


```
new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth

new Oid("2.5.29.35") // authorityKeyIdentifier

new Oid("2.5.29.19") // - Basic Constraints
```

ref:
https://access.redhat.com/documentation/en-US/Red_Hat_Certificate_System/8.0/html/Admin_Guide/Standard_X.509_v3_Certificate_Extensions.html

#### 2.5.29.35 authorityKeyIdentifier
The Authority Key Identifier extension identifies the public key corresponding to the private key used to sign a certificate. This extension is useful when an issuer has multiple signing keys, such as when a CA certificate is renewed.

The extension consists of one or both of the following:
- An explicit key identifier, set in the keyIdentifier field
- An issuer, set in the authorityCertIssuer field, and serial number, set in the authorityCertSerialNumber field, identifying a certificate

If the keyIdentifier field exists, it is used to select the certificate with a matching subjectKeyIdentifier extension. If the authorityCertIssuer and authorityCertSerialNumber fields are present, then they are used to identify the correct certificate by issuer and serialNumber.

If this extension is not present, then the issuer name alone is used to identify the issuer certificate.
PKIX Part 1 requires this extension for all certificates except self-signed root CA certificates. Where a key identifier has not been established, PKIX recommends that the authorityCertIssuer and authorityCertSerialNumber fields be specified. These fields permit construction of a complete certificate chain by matching the SubjectName and CertificateSerialNumber fields in the issuer's certificate against the authortiyCertIssuer and authorityCertSerialNumber in the Authority Key Identifier extension of the subject certificate.
OID

#### 2.5.29.19 basicConstraints
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


## How to create a self signed client server certificate for mtls

## How to create a chained client server certificate for mtls

## Configuring a Kestrel app for certificate authentication

## Configuring an IIS app for certificate authentication

## Configuring an Azure app for certificate authentication

## PKI public key infrastructure

## Ways of creating certificates: powershell, openssl, C# code

# Links

https://tools.ietf.org/html/rfc5280

https://github.com/rwatjen/AzureIoTDPSCertificates

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-x509ca-overview

https://github.com/Azure/azure-iot-sdk-c/blob/master/tools/CACertificates/CACertificateOverview.md

http://oid-info.com/

https://www.alvestrand.no/objectid/


https://github.com/damienbod/AspNetCoreCertificateAuth

https://github.com/damienbod/Secure_gRpc

https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth

https://docs.microsoft.com/en-us/archive/blogs/kaushal/various-ssltls-certificate-file-typesextensions

https://access.redhat.com/documentation/en-US/Red_Hat_Certificate_System/8.0/html/Admin_Guide/Standard_X.509_v3_Certificate_Extensions.html

### mtls

https://tools.ietf.org/html/draft-ietf-oauth-mtls-14

https://identityserver4.readthedocs.io/en/latest/topics/mtls.html

https://tools.ietf.org/html/rfc7519

https://openid.net/specs/openid-connect-core-1_0.html

https://openid.net/specs/draft-jones-json-web-token-07.html 


### Public Key Infrastructure - PKI

https://gist.github.com/Soarez/9688998

https://github.com/PKISharp/ACMESharpCore