using System;
using System.Collections.Generic;
using System.Text;

namespace CertificateManager.Models
{
    public class ValidityPeriod
    {
        public DateTimeOffset ValidFrom { get; set; }
        public DateTimeOffset ValidTo { get; set; }
    }
}
