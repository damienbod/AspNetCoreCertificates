# Certificate Manager Documentation

Certificate Manager is a package which makes it easy to create certificates (chained and self signed) which can be used to in client server authentication and IoT Devices like Azure IoT Hub

- [Basic usage ASP.NET Core, NET Core](#basic-usage-aspnet-core-net-core)
- [Certificate Configuration](#certificate-configuration )
- [Creating Self Signed Certificates for Client Server Authentication](#creating-self-signed-certificates-for-client-server-authentication)
- [Creating Chained Certificates for Client Server Authentication](#creating-chained-certificates-for-client-server-authentication)
- [Creating Chained Certificates for Azure IoT Hub](#creating-chained-certificates-for-azure-iot-hub)
- [Creating Verify Certificate for Azure IoT Hub](#creating-verify-certificate-for-azure-iot-hub)
- [Creating Device (Leaf) Certificates for Azure IoT Hub](#creating-device-leaf-certificates-for-azure-iot-hub)
- [Creating certificates for application development Angular, VUE.js](#creating-certificates-for-application-development-angular-vuejs)
- [Exporting Certificates](#exporting-certificates)
- [General Certificates, full APIs](#general-certificates-full-apis)
- [Certificate Configuration full APIs](#certificate-configuration-full-apis)

# Basic usage ASP.NET Core, .NET Core

Add the NuGet package to the your project file

```
<PackageReference Include="CertificateManager" Version="1.0.9" />
```

The NuGet packages uses dependency injection to setup. In a console application initialize the package as follows:

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

```

Or in an ASP.NET Core application use the Startup ConfigureServices method to initialize the package.

```csharp
builder.Services.AddCertificateManager();
```

## Certificate Configuration

### Distinguished Name

The distinguished name will be saved to the Issuer and the Subject properties of the certificate.

```csharp
var distinguishedName = new DistinguishedName
{
    CommonName = "localhost",
    Country = "CH",
    Locality = "CH",
    Organisation = "damienbod",
    OrganisationUnit = "development"
};
```
The CommonName property is required.

example in certificate:
```
C=CH, C=CH, O=damienbod, OU=development, CN=localhost
```

definitions:

- C= Country 
- ST= State or province
- L= Locality
- O= organisation
- OU=Organisation Unit
- CN= Common name 

// CN is REQUIRED

### Validity Period

The Validity Period defines when the certificate is valid from and how long.

```csharp
var validityPeriod = new ValidityPeriod
{
    ValidFrom = DateTime.UtcNow,
    ValidTo = DateTime.UtcNow.AddYears(10)
};
```

If creating a child certificate from a root or an intermediate certification, the values cannot be outside the range of the parent. If the certificate values are outside the range, the parent values will be used.

The ValidFrom and the ValidTo values can then be used to validate the certificate. It is recommended the keep this period short. This depends on how you use and deploy the certificates.

## Creating Self Signed Certificates for Client Server Authentication

The **CreateCertificatesClientServerAuth** service is used to create these certificates.

```csharp
var dnsName = "localhost";
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();
```

The **NewServerSelfSignedCertificate** method can be used to create a self signed certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

Oid("1.3.6.1.5.5.7.3.1"), // TLS Server auth

This can then be validated.

```csharp
// Server self signed certificate
var server = createClientServerAuthCerts.NewServerSelfSignedCertificate(
    new DistinguishedName { CommonName = "server", Country = "CH" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    dnsName);
server.FriendlyName = "azure server certificate";
```

The **NewClientSelfSignedCertificate** method can be used to create a self signed certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth

This can then be validated.

```csharp
// Client self signed certificate
var client = createClientServerAuthCerts.NewClientSelfSignedCertificate(
    new DistinguishedName { CommonName = "client", Country = "CH" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    dnsName);

client.FriendlyName = "azure client certificate";
```

## Creating Chained Certificates for Client Server Authentication

The **CreateCertificatesClientServerAuth** service is used to create these certificates.

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();
```

The **NewRootCertificate** method creates a new root certificate which can be used for chained structures. If you use your own root certificate, it needs to be added to the trusted certificate store on deployment host.

This is not needed if creting certificates from a public CA certificate. The root certificatge is a self signed certificate.

```csharp
var rootCaL1 = createClientServerAuthCerts.NewRootCertificate(
    new DistinguishedName { CommonName = "root dev", Country = "IT" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    3, "localhost");
rootCaL1.FriendlyName = "developement root L1 certificate";
```

The **NewIntermediateChainedCertificate** creates an intermediate certificate from a parent root or another intermediate certificate. 

```csharp
// Intermediate L2 chained from root L1
var intermediateCaL2 = createClientServerAuthCerts.NewIntermediateChainedCertificate(
    new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    2,  "localhost", rootCaL1);
intermediateCaL2.FriendlyName = "developement Intermediate L2 certificate";
```

The **NewServerChainedCertificate** method can be used to create a self signed certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

Oid("1.3.6.1.5.5.7.3.1"), // TLS Server auth

This can then be validated.

```csharp
// Server, Client L3 chained from Intermediate L2
var serverL3 = createClientServerAuthCerts.NewServerChainedCertificate(
    new DistinguishedName { CommonName = "server", Country = "DE" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    "localhost", intermediateCaL2);
serverL3.FriendlyName = "developement server L3 certificate";
```

The **NewClientChainedCertificate** method can be used to create a chained certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth

This can then be validated.

```csharp
var clientL3 = createClientServerAuthCerts.NewClientChainedCertificate(
    new DistinguishedName { CommonName = "client", Country = "IE" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    "localhost", intermediateCaL2);
clientL3.FriendlyName = "developement client L3 certificate";
            
```

## Creating Chained Certificates for Azure IoT Hub

```csharp
var serviceProvider = new ServiceCollection()
	.AddCertificateManager()
	.BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

var root = createClientServerAuthCerts.NewRootCertificate(
	new DistinguishedName { CommonName = "root dev", Country = "IT" },
	new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
	3, "localhost");
root.FriendlyName = "developement root certificate";

// Intermediate L2 chained from root L1
var intermediate = createClientServerAuthCerts.NewIntermediateChainedCertificate(
	new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
	new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
	2, "localhost", root);
intermediate.FriendlyName = "developement Intermediate certificate";

string password = "1234";
var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, root);
File.WriteAllBytes("root.pfx", rootCertInPfxBtyes);

// https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-x509-get-started

var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(root);
var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
File.WriteAllBytes($"root.cer", rootPublicKeyBytes);

var intermediateCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, intermediate, root);
File.WriteAllBytes("intermediate.pfx", intermediateCertInPfxBtyes);
```

## Creating Verify Certificate for Azure IoT Hub

```csharp
var serviceProvider = new ServiceCollection()
	.AddCertificateManager()
	.BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

var root = X509CertificateLoader.LoadPkcs12FromFile("root.pfx", "1234");

var deviceVerify = createClientServerAuthCerts.NewDeviceVerificationCertificate(
"<veification code from Azure IoT Hub>", root);
deviceVerify.FriendlyName = "device verify";

var deviceVerifyPEM = importExportCertificate.PemExportPublicKeyCertificate(deviceVerify);
File.WriteAllText("deviceVerify.pem", deviceVerifyPEM);

var deviceVerifyPublicKey = importExportCertificate.ExportCertificatePublicKey(deviceVerify);
var deviceVerifyPublicKeyBytes = deviceVerifyPublicKey.Export(X509ContentType.Cert);
File.WriteAllBytes($"deviceVerify.cer", deviceVerifyPublicKeyBytes);

```

## Creating Device (Leaf) Certificates for Azure IoT Hub

```csharp
var serviceProvider = new ServiceCollection()
	.AddCertificateManager()
	.BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

var intermediate = X509CertificateLoader.LoadPkcs12FromFile("intermediate.pfx", "1234");

var testDevice01 = createClientServerAuthCerts.NewDeviceChainedCertificate(
	new DistinguishedName { CommonName = "<Device ID>" },
	new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
	"localhost", intermediate);
testDevice01.FriendlyName = "IoT device testDevice01";

string password = "1234";
var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

var deviceInPfxBytes = importExportCertificate.ExportChainedCertificatePfx(password, testDevice01, intermediate);
File.WriteAllBytes("testDevice01.pfx", deviceInPfxBytes);
```

## Creating certificates for application development Angular, VUE.js

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var _createCertificatesRsa = serviceProvider.GetService<CreateCertificatesRsa>();

// Create development certificate for localhost
var devCertificate = _createCertificatesRsa
    .CreateDevelopmentCertificate("localhost", 10);

devCertificate.FriendlyName = "localhost development";

string password = "1234";
var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

// full pfx with password
var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, devCertificate);
File.WriteAllBytes("dev_localhost.pfx", rootCertInPfxBtyes);

// private key
var exportRsaPrivateKeyPem = importExportCertificate.PemExportRsaPrivateKey(devCertificate);
File.WriteAllText($"dev_localhost.key", exportRsaPrivateKeyPem);

// public key certificate as pem
var exportPublicKeyCertificatePem = importExportCertificate.PemExportPublicKeyCertificate(devCertificate);
File.WriteAllText($"dev_localhost.pem", exportPublicKeyCertificatePem);
```

## Creating Chained Certificates from a trusted CA Certificate

## Exporting Certificates

### Exporting self signed certificates

```csharp
var serverCertInPfxBtyes = 
    importExportCertificate.ExportSelfSignedCertificatePfx(password, server);
File.WriteAllBytes("server.pfx", serverCertInPfxBtyes);

var clientCertInPfxBtyes = 
    importExportCertificate.ExportSelfSignedCertificatePfx(password, client);
File.WriteAllBytes("client.pfx", clientCertInPfxBtyes);
```
### Exporting chained certificates

```csharp
string password = "1234";
var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

var rootCertInPfxBtyes = importExportCertificate.ExportRootPfx(password, rootCaL1);
File.WriteAllBytes("localhost_root_l1.pfx", rootCertInPfxBtyes);

var rootPublicKey = importExportCertificate.ExportCertificatePublicKey(rootCaL1);
var rootPublicKeyBytes = rootPublicKey.Export(X509ContentType.Cert);
File.WriteAllBytes($"localhost_root_l1.cer", rootPublicKeyBytes);

var intermediateCertInPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, intermediateCaL2, rootCaL1);
File.WriteAllBytes("localhost_intermediate_l2.pfx", intermediateCertInPfxBtyes);

var serverCertL3InPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, serverL3, intermediateCaL2);
File.WriteAllBytes("serverl3.pfx", serverCertL3InPfxBtyes);

var clientCertL3InPfxBtyes = importExportCertificate.ExportChainedCertificatePfx(password, clientL3, intermediateCaL2);
File.WriteAllBytes("clientl3.pfx", clientCertL3InPfxBtyes);
```

### Exporting verify certificates

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

var importExportCertificate = serviceProvider.GetService<ImportExportCertificate>();

var root = X509CertificateLoader.LoadPkcs12FromFile("root.pfx", "1234");

var deviceVerify = createClientServerAuthCerts.NewDeviceVerificationCertificate(
"4C8C754C6DA4280DBAB7FC7BB320E7FFFB7F411CBB7EAA7D", root);
deviceVerify.FriendlyName = "device verify";

var deviceVerifyPEM = importExportCertificate.PemExportPublicKeyCertificate(deviceVerify);
File.WriteAllText("deviceVerify.pem", deviceVerifyPEM);

var deviceVerifyPublicKey = importExportCertificate.ExportCertificatePublicKey(deviceVerify);
var deviceVerifyPublicKeyBytes = deviceVerifyPublicKey.Export(X509ContentType.Cert);
File.WriteAllBytes($"deviceVerify.cer", deviceVerifyPublicKeyBytes);
```

## Exporting Importing PEM

RSA

```csharp
var sp = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var ccRsa = sp.GetService<CreateCertificatesRsa>();
var iec = sp.GetService<ImportExportCertificate>();

var rsaCert = ccRsa.CreateDevelopmentCertificate("localhost", 2, 2048);

// export
var publicKeyPem = iec.PemExportPublicKeyCertificate(rsaCert);
var rsaPrivateKeyPem = iec.PemExportRsaPrivateKey(rsaCert);

// import
var roundTripPublicKeyPem = iec.PemImportCertificate(publicKeyPem);
var roundTripRsaPrivateKeyPem = iec.PemImportPrivateKey(rsaPrivateKeyPem);

var roundTripFullCert = 
    iec.CreateCertificateWithPrivateKey(
        roundTripPublicKeyPem, 
        roundTripRsaPrivateKeyPem, 
        "1234");

```

ECDsa

```csharp
var sp = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var cc = serviceProvider.GetService<CreateCertificatesClientServerAuth>();

var root = cc.NewRootCertificate(
    new DistinguishedName { CommonName = "root dev", Country = "IT" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    3, "localhost");
root.FriendlyName = "developement root L1 certificate";

var iec = sp.GetService<ImportExportCertificate>();

// export
var publicKeyPem = iec.PemExportPublicKeyCertificate(root);
var eCDsaPrivateKeyPem = iec.PemExportECPrivateKey(root);

// import
var roundTripPublicKeyPem = iec.PemImportCertificate(publicKeyPem);
var roundTripECPrivateKeyPem = iec.PemImportPrivateKey(eCDsaPrivateKeyPem);

var roundTripFullCert = 
    iec.CreateCertificateWithPrivateKey(
        roundTripPublicKeyPem, 
        roundTripECPrivateKeyPem, 
        "1234");

```
## General Certificates, full APIs

### Self signed certificate

Creating a self signed certificate using **NewECDsaSelfSignedCertificate** with ECDsa

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var enhancedKeyUsages = new OidCollection {
    new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
    new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
};

var createCertificates = serviceProvider.GetService<CreateCertificates>();

// Create the root self signed cert
var rootCert = createCertificates.NewECDsaSelfSignedCertificate(
    RootCertConfig.DistinguishedName,
    RootCertConfig.BasicConstraints,
    RootCertConfig.ValidityPeriod,
    RootCertConfig.SubjectAlternativeName,
    enhancedKeyUsages,
    RootCertConfig.X509KeyUsageFlags,
    new ECDsaConfiguration());

rootCert.FriendlyName = "localhost root l1";
```

Certificate configuration for a self signed root

```csharp
public static class RootCertConfig
{
    public static DistinguishedName DistinguishedName = new DistinguishedName
    {
        CommonName = "localhost",
        Country = "CH",
        Locality = "CH",
        Organisation = "damienbod",
        OrganisationUnit = "developement"
    };

    public static BasicConstraints BasicConstraints = new BasicConstraints
    {
        CertificateAuthority = true,
        HasPathLengthConstraint = true,
        PathLengthConstraint = 3,
        Critical = true
    };

    public static ValidityPeriod ValidityPeriod = new ValidityPeriod
    {
        ValidFrom = DateTime.UtcNow,
        ValidTo = DateTime.UtcNow.AddYears(10)
    };

    public static SubjectAlternativeName SubjectAlternativeName = new SubjectAlternativeName
    {
        Email = "damienbod@damienbod.ch",
        DnsName = new List<string>
        {
            "localhost",
            "test.damienbod.ch"
        }
    };

    // Only X509KeyUsageFlags.KeyCertSign required for client server auth
    public static X509KeyUsageFlags X509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature
            | X509KeyUsageFlags.KeyEncipherment
            | X509KeyUsageFlags.KeyCertSign;
    }
```

### Chained certificate

Creating a certificate using **NewECDsaChainedCertificate** using ECDsa

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var enhancedKeyUsages = new OidCollection {
    new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
    new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
};

var createCertificates = serviceProvider.GetService<CreateCertificates>();

var deviceCertificate = createCertificates.NewECDsaChainedCertificate(
    DeviceCertConfig.DistinguishedName,
    DeviceCertConfig.BasicConstraints,
    DeviceCertConfig.ValidityPeriod,
    DeviceCertConfig.SubjectAlternativeName,
    intermediateCertificateLevel3,
    enhancedKeyUsages,
    DeviceCertConfig.X509KeyUsageFlags,
    new ECDsaConfiguration());

deviceCertificate.FriendlyName = "device cert l4";
```

Device certificate chained

```csharp
public static class DeviceCertConfig
    {
        public static DistinguishedName DistinguishedName = new DistinguishedName
        {
            CommonName = "localhost",
            Country = "CH",
            Locality = "CH",
            Organisation = "firma x",
            OrganisationUnit = "skills"
        };

        public static BasicConstraints BasicConstraints = new BasicConstraints
        {
            CertificateAuthority = false,
            HasPathLengthConstraint = false,
            PathLengthConstraint = 0,
            Critical = true
        };

        public static ValidityPeriod ValidityPeriod = new ValidityPeriod
        {
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddYears(10)
        };

        public static SubjectAlternativeName SubjectAlternativeName = new SubjectAlternativeName
        {
            DnsName = new List<string>
            {
                "localhost"
            }
        };

        public static X509KeyUsageFlags X509KeyUsageFlags = 
             X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;
    }
```

## Certificate Configuration full APIs

### Basic Constraints

basic constraints for intermediate and root certificates. Set the path length to match the chain length.

```csharp
var basicConstraints = new BasicConstraints
{
    CertificateAuthority = true,
    HasPathLengthConstraint = true,
    PathLengthConstraint = pathLengthConstraint,
    Critical = true
};
```

Or the basic constaints for a device, client or server certificate.

```csharp
var basicConstraints = new BasicConstraints
{
    CertificateAuthority = false,
    HasPathLengthConstraint = false,
    PathLengthConstraint = 0,
    Critical = true
};
```

### Subject Alternative Name

Add the required or supported DnsName or the Email here.

```csharp
var subjectAlternativeName = new SubjectAlternativeName
{
    DnsName = new List<string>
    {
        "localhost"
    }
};
```

### Enhanced Key Usages

Defines how the certificate key can be used. 

```csharp
var enhancedKeyUsages = new OidCollection {
    new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
    new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
};
```

- new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
- new Oid("1.3.6.1.5.5.7.3.2")  // TLS Client auth
- new Oid("1.3.6.1.5.5.7.3.3")  // Code signing 
- new Oid("1.3.6.1.5.5.7.3.4")  // Email
- new Oid("1.3.6.1.5.5.7.3.8")  // Timestamping  

### X509 Key Usage Flags

Defines how the certificate key can be used. 

```csharp
var x509KeyUsageFlags = 
             X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;
```

- None             No key usage parameters.
- EncipherOnly     The key can be used for encryption only.
- CrlSign          The key can be used to sign a certificate revocation list (CRL).
- KeyCertSign      The key can be used to sign certificates.
- KeyAgreement     The key can be used to determine key agreement, such as a key created using the Diffie-Hellman key agreement algorithm.
- DataEncipherment The key can be used for data encryption.
- KeyEncipherment  The key can be used for key encryption.
- NonRepudiation   The key can be used for authentication.
- DecipherOnly     The key can be used for decryption only.
