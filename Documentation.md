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

## Certification Configuration

### Distinguished Name

```
var distinguishedName = new DistinguishedName
{
    CommonName = "localhost",
    Country = "CH",
    Locality = "CH",
    Organisation = "damienbod",
    OrganisationUnit = "developement"
};
```
The CommonName and the Country properties are required.

example in certificate:
```
C=CH, C=CH, O=damienbod, OU=developement, CN=localhost
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

## Creating Self Signed Certificates for Client Server Authentication

## Creating Self Signed Certificates for Azure Client Server Authentication

## Creating Chained Certificates for Client Server Authentication

## Creating Chained Certificates for Azure IoT 

## Creating Chained Certificates from a trusted CA Certificate

## Exporting Certificates

