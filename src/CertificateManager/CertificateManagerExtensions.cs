using CertificateManager;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CertificateManagerExtensions
    {
        public static IServiceCollection AddCertificateManager(this IServiceCollection services)
        {
            // internal
            services.AddTransient<CertificateUtility>();
            services.AddTransient<PemParser>();

            // public
            services.AddTransient<ImportExportCertificate>();
            services.AddTransient<CreateCertificates>();
            services.AddTransient<CreateCertificatesClientServerAuth>();

            return services;
        }
    }
}
