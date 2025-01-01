using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuth;

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

        services.AddHttpClient();
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
        });

        var chainedClient = X509CertificateLoader.LoadPkcs12FromFile("../Certs/clientl4.pfx", "1234");
        var handlerChainedClient = new HttpClientHandler();
        handlerChainedClient.ClientCertificates.Add(chainedClient);

        services.AddHttpClient("chained_client", c => { })
            .ConfigurePrimaryHttpMessageHandler(() => handlerChainedClient);

        services.AddRazorPages();

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
        app.UseStaticFiles();

        app.UseCookiePolicy();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();

        return app;
    }
}