using CertificateManager;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CertificateManagerExtensions
    {
        public static IServiceCollection AddCertificateManager(this IServiceCollection services)
        {
            // internal
            services.AddTransient<CertificateUtility>();
            services.AddTransient<IntermediateCertificate>();
            services.AddTransient<RootCertificate>();
            services.AddTransient<DeviceCertificate>();

            // public
            services.AddTransient<ImportExportCertificate>();
            services.AddTransient<CreateCertificates>();
            services.AddTransient<CreateCertificatesClientServerAuth>();

            return services;
        }
    }
}
