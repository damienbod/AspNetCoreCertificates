# Certificate Manager Documentation

Certificate Manager is a package which makes it easy to create certifcates which can be used to in client server authentication and IoT Devices like Azure IoT Hub

# Basic usage ASP.NET Core, NET Core

Add the NuGet package to the your project file

```
<PackageReference Include="CertificateManager" Version="1.0.0" />
```

The NuGet packages uses dependency injection to setup. In a console application initialize the package as follows:

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

```

Or in an ASP.NET Core application use the Startup ConfigureServices method to initialize the package.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddCertificateManager();
}
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
The CommonName and the Country properties are required.

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
- CN= Common name (DNS) 

// C and CN are REQUIRED

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

The CreateCertificatesClientServerAuth service is used to create these certificates.

```csharp
var dnsName = "localhost";
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();
```

The NewServerSelfSignedCertificate method can be used to create a self signed certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

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

The NewClientSelfSignedCertificate method can be used to create a self signed certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

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

The CreateCertificatesClientServerAuth service is used to create these certificates.

```csharp
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

var createClientServerAuthCerts = serviceProvider.GetService<CreateCertificatesClientServerAuth>();
```

The NewRootCertificate method creates a new root certificate which can be used for chained structures. If you use your own root certificate, it needs to be added to the trusted certificate store on deployment host.

This is not needed if creting certificates from a public CA certificate. The root certificatge is a self signed certificate.

```csharp
var rootCaL1 = createClientServerAuthCerts.NewRootCertificate(
    new DistinguishedName { CommonName = "root dev", Country = "IT" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    3, "localhost");
rootCaL1.FriendlyName = "developement root L1 certificate";
```

The NewIntermediateChainedCertificate creates an intermediate certificate from a parent root or another intermediate certificate. 

```csharp
// Intermediate L2 chained from root L1
var intermediateCaL2 = createClientServerAuthCerts.NewIntermediateChainedCertificate(
    new DistinguishedName { CommonName = "intermediate dev", Country = "FR" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    2,  "localhost", rootCaL1);
intermediateCaL2.FriendlyName = "developement Intermediate L2 certificate";
```

The NewServerChainedCertificate method can be used to create a self signed certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

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

The NewClientChainedCertificate method can be used to create a chained certificate for a certificate which is to be used on the server. The dnsName must match your server deployment. Only the correct enhanced Key usages is set. 

Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth

This can then be validated.
```csharp
var clientL3 = createClientServerAuthCerts.NewClientChainedCertificate(
    new DistinguishedName { CommonName = "client", Country = "IE" },
    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(10) },
    "localhost", intermediateCaL2);
clientL3.FriendlyName = "developement client L3 certificate";
            
```

## Creating Chained Certificates for Azure IoT 

## Creating Chained Certificates from a trusted CA Certificate

## Exporting Certificates

