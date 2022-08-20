using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AzureCertAuth;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
       .MinimumLevel.Debug()
       .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
       .Enrich.FromLogContext()
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
                .WriteTo.File(
                    //$@"../certauth.txt",
                    $@"D:\home\LogFiles\Application\{Environment.UserDomainName}.txt",
                    fileSizeLimitBytes: 1_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            )
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
