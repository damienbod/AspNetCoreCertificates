# Certificate Manager Readme

// TODO can setup once the repo is public 

|                           | Build                                                                                                                                                       | Certificate Manager                                                                                                                                |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| .NET Core                 | [![Build status](https://ci.appveyor.com/api/projects/status/gyychgc7l5g4g5lb?svg=true)](https://ci.appveyor.com/project/damienbod/aspnet5localization)      | [![NuGet Status](http://img.shields.io/nuget/v/Localization.SqlLocalizer.svg?style=flat-square)](https://www.nuget.org/packages/Localization.SqlLocalizer/) |


========================

<strong>Basic Usage ASP.NET Core</strong>

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

Now the package is ready to use. See the Documentation to create the specific certificates for your use case.

# Examples Creating Certificates:

<ul>
    <li><a href="https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateChainedCertsConsoleDemo">Create Chained Certs Console</a></li></ul>

# Examples Using Certificates:

<ul>
    <li><a href="https://github.com/damienbod/AspNetCoreCertificates/tree/master/examplesUsingCertificateAuthentication/AspNetCoreChained">ASP.NET Core Chained</a></li>
    <li><a href="https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth">Microsoft Docs: Configure certificate authentication in ASP.NET Core</a></li>
</ul>

