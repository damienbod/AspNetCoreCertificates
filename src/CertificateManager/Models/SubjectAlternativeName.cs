using System.Collections.Generic;

namespace CertificateManager.Models
{
    public class SubjectAlternativeName
    {
        /// <summary>
        /// At least 1 is required, and must match your deployment
        /// For example for local development, localhost 
        /// </summary>
        public List<string> DnsName { get; set; } = new List<string>();

        /// <summary>
        /// optional
        /// </summary>
        public string Email { get; set; }
    }
}
