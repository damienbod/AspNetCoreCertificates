using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CertificatesCreation.Models
{
    /// <summary>
    /// Certificate Distinguished Name
    /// C and CN are REQUIRED
    /// CN should be your DNS
    /// </summary>
    public class DistinguishedName
    {
        /// <summary>
        /// C= Country
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// ST = State or province
        /// </summary>
        public string StateProvince { get; set; }

        /// <summary>
        /// L= Locality
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// O = organisation
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// OU=Organisation Unit
        /// </summary>
        public string OrganisationUnit { get; set; }

        /// <summary>
        ///  CN= Common name(DNS)
        /// </summary>
        [Required]
        public string CommonName { get; set; }

    }

}
