using AspNetCoreCertificateAuthApi;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuthApiChained;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        if (builder.Environment.IsDevelopment())
        {
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.ConfigureHttpsDefaults(listenOptions =>
                {
                    listenOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    listenOptions.AllowAnyClientCertificate();
                });
            });
        }

        services.AddSingleton<MyCertificateValidationService>();

        services.AddCertificateForwarding(options =>
        {
            options.CertificateHeader = "X-ARR-ClientCert";
            options.HeaderConverter = (headerValue) =>
            {
                Console.WriteLine("headerValue: " + headerValue);

                X509Certificate2? clientCertificate = null;
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    byte[] bytes = Convert.FromBase64String(headerValue);
                    clientCertificate = X509CertificateLoader.LoadCertificate(bytes);
                }

                return clientCertificate!;
            };
        });

        services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(options => // code from ASP.NET Core sample
            {
                // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth
                options.AllowedCertificateTypes = CertificateTypes.All;

                // Default values
                //options.AllowedCertificateTypes = CertificateTypes.Chained;
                //options.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                options.RevocationMode = X509RevocationMode.NoCheck;
                options.ValidateCertificateUse = false;
                options.ValidateValidityPeriod = false;

                options.Events = new CertificateAuthenticationEvents
                {
                    OnCertificateValidated = context =>
                    {
                        var validationService =
                            context.HttpContext.RequestServices.GetService<MyCertificateValidationService>();

                        if (validationService!.ValidateCertificate(context.ClientCertificate))
                        {
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                            };

                            context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                            context.Success();
                        }
                        else
                        {
                            context.Fail("invalid cert");
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = new Func<CertificateAuthenticationFailedContext, Task>(context =>
                    {
                        Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    })
                };
            });

        services.AddAuthorization();
        services.AddControllers();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        if (app.Environment.IsDevelopment())
        {
            app.UseCertificateForwarding();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}