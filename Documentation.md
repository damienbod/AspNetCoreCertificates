# Certificate Manager Documentation

Certificate Manager is a package which makes it easy to create certifcates which can be used to in client server authentication and IoT Devices like Azure IoT Hub

# Basic Usage ASP.NET Core

Add the NuGet package to the your project file

```
<PackageReference Include="CertificateManager" Version="1.0.0" />
```

The NuGet packages uses dependency injection to setup. In a console application initialize the package as follows:
```
var serviceProvider = new ServiceCollection()
    .AddCertificateManager()
    .BuildServiceProvider();

```

Or in an ASP.NET Core application use the Startup ConfigureServices method to initialize the package.

```
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddCertificateManager();
}
```

## Certificate Configuration

### Distinguished Name

The distinguished name will be saved to the Issuer and the Subject properties of the certificate.

```
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

```
var validityPeriod = new ValidityPeriod
{
    ValidFrom = DateTime.UtcNow,
    ValidTo = DateTime.UtcNow.AddYears(10)
};
```

If creating a child certificate from a root or an intermediate certification, the values cannot be outside the range of the parent. If the certificate values are outside the range, the parent values will be used.

The ValidFrom and the ValidTo values can then be used to validate the certificate. It is recommended the keep this period short. This depends on how you use and deploy the certificates.

## Creating Self Signed Certificates for Client Server Authentication

## Creating Self Signed Certificates for Azure Client Server Authentication

## Creating Chained Certificates for Client Server Authentication

## Creating Chained Certificates for Azure IoT 

## Creating Chained Certificates from a trusted CA Certificate

## Exporting Certificates

