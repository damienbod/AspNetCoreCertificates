using System;

namespace CertificateManager.Models
{
    public class ValidityPeriod
    {
        public DateTimeOffset ValidFrom { get; set; }
        public DateTimeOffset ValidTo { get; set; }
    }
}
