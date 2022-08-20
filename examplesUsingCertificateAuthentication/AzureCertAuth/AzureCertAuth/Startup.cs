using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Certificate;

namespace AzureCertAuth;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MyCertificateValidationService>();

        services.AddCertificateForwarding(options =>
        {
            options.CertificateHeader = "X-ARR-ClientCert";
            options.HeaderConverter = (headerValue) =>
            {

                X509Certificate2? clientCertificate = null;
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    byte[] bytes = Convert.FromBase64String(headerValue);
                    clientCertificate = new X509Certificate2(bytes);
                }

                return clientCertificate;
            };
        });

        services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(options => // code from ASP.NET Core sample
            {
                // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth
                options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                
                // Default values
                //options.AllowedCertificateTypes = CertificateTypes.Chained;
                //options.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                //options.RevocationMode = X509RevocationMode.Online;
                //options.ValidateCertificateUse = true;
                //options.ValidateValidityPeriod = true;

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
                    }
                };
            });

        services.AddAuthorization();
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        //app.UseCertificateForwarding();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
