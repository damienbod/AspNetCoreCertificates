using System;
using System.Collections.Generic;
using System.Text;

namespace CertificateManager
{
    public class CertificateManagerService
    {
        private readonly IntermediateCertificate _intermediateCertificate;
        private readonly RootCertificate _rootCertificate;
        private readonly DeviceCertificate _deviceCertificate;

        public CertificateManagerService(
            IntermediateCertificate intermediateCertificate,
            RootCertificate rootCertificate,
            DeviceCertificate deviceCertificate)
        {
            _intermediateCertificate = intermediateCertificate;
            _rootCertificate = rootCertificate;
            _deviceCertificate = deviceCertificate;
        }

    }
}
