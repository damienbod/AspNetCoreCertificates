using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AspNetCoreCertificateAuth;

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
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                       .ReadFrom.Configuration(hostingContext.Configuration)
                       .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                       .MinimumLevel.Verbose()
                       .Enrich.FromLogContext()
                       .WriteTo.File("../_chainedServerLogs.txt")
                       .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            )
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
