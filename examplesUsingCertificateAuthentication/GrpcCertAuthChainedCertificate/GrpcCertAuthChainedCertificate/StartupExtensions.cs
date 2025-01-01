using GrpcCertAuthChainedCertificate;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace DownstreamApiCertAuth;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var cert = X509CertificateLoader.LoadPkcs12FromFile(Path.Combine("../Certs/serverl4.pfx"), "1234");

        if (builder.Environment.IsDevelopment())
        {
            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.Limits.MinRequestBodyDataRate = null;
                options.ListenLocalhost(44379, listenOptions =>
                {
                    listenOptions.UseHttps(cert);
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    listenOptions.UseConnectionLogging();
                });

                options.ConfigureHttpsDefaults(listenOptions =>
                {
                    listenOptions.ServerCertificate = cert;
                    listenOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    listenOptions.AllowAnyClientCertificate();


                });
            });
        }

        services.AddAuthorization();
        services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(options =>
            {
                // Not recommended in production environments. The example is using a self-signed test certificate.
                options.RevocationMode = X509RevocationMode.NoCheck;
                options.AllowedCertificateTypes = CertificateTypes.Chained;
            });

        services.AddGrpc();

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

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGrpcService<GreeterService>();

        app.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        });


        return app;
    }
}