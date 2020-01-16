using CertificateManager;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CertificateManagerExtensions
    {
        public static IServiceCollection AddCertificateManager(this IServiceCollection services)
        {
            services.AddTransient<CertificateUtility>();
            services.AddTransient<ImportExportCertificate>();
            services.AddTransient<IntermediateCertificate>();
            services.AddTransient<RootCertificate>();
            services.AddTransient<DeviceCertificate>();
            services.AddTransient<CertificateManagerService>();

            return services;
        }
    }
}
