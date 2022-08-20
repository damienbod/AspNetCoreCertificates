using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AspNetCoreCertificateAuthApi;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var builder = config.Build();
                var keyVaultEndpoint = builder["AzureKeyVaultEndpoint"];
                IHostEnvironment env = context.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                //.AddUserSecrets("your user secret....");

            })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                   .ReadFrom.Configuration(hostingContext.Configuration)
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                   .MinimumLevel.Verbose()
                   .Enrich.FromLogContext()
                   .WriteTo.File("../_chainedClientLogs.txt")
                   .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            )
           .ConfigureWebHostDefaults(webBuilder =>
           {
               var cert = new X509Certificate2(Path.Combine("../Certs/serverl4.pfx"), "1234");
               webBuilder.UseStartup<Startup>()
                   .ConfigureKestrel(options =>
                   {
                       options.Limits.MinRequestBodyDataRate = null;
                       options.ConfigureHttpsDefaults(o =>
                        {
                            o.ServerCertificate = cert;
                            o.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                        });
                       options.ListenLocalhost(44378, listenOptions =>
                       {
                           listenOptions.UseHttps(cert);
                           listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                           listenOptions.UseConnectionLogging();

                       });

                   });
            });
}
