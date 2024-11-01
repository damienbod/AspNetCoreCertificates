**Certificate Manager** is a package which makes it easy to create certificates which can be used to in client server authentication and IoT Devices like Azure IoT Hub

|                           | Build                                                                                                                                                       | Certificate Manager                                                                                                                                |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| .NET Core                 | [![.NET](https://github.com/damienbod/AspNetCoreCertificates/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/AspNetCoreCertificates/actions/workflows/dotnet.yml)     | [![NuGet Status](http://img.shields.io/nuget/v/CertificateManager.svg?style=flat-square)](https://www.nuget.org/packages/CertificateManager/) |

========================

[Quickstart](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateChainedCertsConsoleDemo) | [Documentation](https://github.com/damienbod/AspNetCoreCertificates/blob/master/Documentation.md) | [Changelog](https://github.com/damienbod/AspNetCoreCertificates/blob/master/CHANGELOG.md)

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

Now the package is ready to use. See the [Documentation](https://github.com/damienbod/AspNetCoreCertificates/blob/master/Documentation.md)  to create the specific certificates for your use case.

# Examples Creating Certificates:

- [Create chained certificate authentication certificates console](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateChainedCertsConsoleDemo)
- [Create self signed certificate authentication certificates console](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateSelfSignedCertsConsoleDemo)
- [Create chained certificates for Azure IoT Hub](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/IoTHubCreateChainedCerts)
- [Create verify certificate for Azure IoT Hub .pem or .cer](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/IoTHubVerifyCertificate)
- [Create device (Leaf) certificate for Azure IoT Hub device](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/IoTHubCreateDeviceCertificate)
- [Create development certificates for SPAs HTTPS development, like Vue.js, Angular](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateAngularVueJsDevelopmentCertificates)
- [Create certificates for IdentityServer RSA and ECDsa](https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateIdentityServer4Certificates)


# Examples Using Certificates:

- [ASP.NET Core chained certificate authentication](https://github.com/damienbod/AspNetCoreCertificates/tree/master/examplesUsingCertificateAuthentication/AspNetCoreChained)
- [Azure ASP.NET Core self signed certificate authentication](https://github.com/damienbod/AspNetCoreCertificates/tree/master/examplesUsingCertificateAuthentication/AzureCertAuth)
- [Grpc chained certificate authentication](https://github.com/damienbod/AspNetCoreCertificates/tree/master/examplesUsingCertificateAuthentication/GrpcCertAuthChainedCertificate)
- [Simulate Azure IoT Hub Device with device certificate](https://github.com/damienbod/AspNetCoreCertificates/tree/master/examplesUsingCertificateAuthentication/SimulateAzureIoTDevice)
- [signing a csr with root ca certificate](https://github.com/damienbod/AspNetCoreCertificates/tree/master/examplesUsingCertificateAuthentication/SigningCertificate)


# Microsoft Certificate Authentication Docs:

- [Microsoft Docs: Configure certificate authentication in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth)

# Read certificates and private keys from PEM files

https://github.com/oocx/ReadX509CertificateFromPem

# Blogs

- [Creating Certificates for X.509 security in Azure IoT Hub using .NET Core](https://damienbod.com/2020/01/29/creating-certificates-for-x-509-security-in-azure-iot-hub-using-net-core/)
- [Creating Certificates in .NET Core for Vue.js development using HTTPS](https://damienbod.com/2020/02/04/creating-certificates-in-net-core-for-vue-js-development-using-https/)
- [Create Certificates for IdentityServer signing using .NET Core](https://damienbod.com/2020/02/10/create-certificates-for-identityserver4-signing-using-net-core/)
- [Provisioning X.509 Devices for Azure IoT Hub using .NET Core](https://damienbod.com/2020/02/20/provisioning-x-509-devices-for-azure-iot-hub-using-net-core)

