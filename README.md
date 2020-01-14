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

Serial number: Unique identifier within a certification authority

Signature algorithm: SHA usually

Issuer: Distinguished Name
 
Subject: Distinguished Name 

Validity period

- Valid From
- Valid To  

### Distinguished Name:

example:
```
C=CH, O=damienbod, OU=testing, CN=localhost
```

### SAN SubjectAlternativeName

## Certificate Types

## Certificate Extentions, OID

### C#
- X509BasicConstraintsExtension
- X509KeyUsageExtension
- X509EnhancedKeyUsageExtension => OID

new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth

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



## How to create a self signed client server certificate for mtls

## How to create a chained client server certificate for mtls

## Configuring a Kestrel app for certificate authentication

## Configuring an IIS app for certificate authentication

## Configuring an Azure app for certificate authentication

## PKI public key infrastructure

## Ways of creating certificates: powershell, openssl, C# code

# Links

https://github.com/rwatjen/AzureIoTDPSCertificates

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-x509ca-overview

https://github.com/Azure/azure-iot-sdk-c/blob/master/tools/CACertificates/CACertificateOverview.md