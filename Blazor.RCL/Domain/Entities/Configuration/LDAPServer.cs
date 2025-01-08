using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Configuration
{
    /// <summary>
    /// Represents an LDAP server configuration in the application.
    /// This entity defines the structure for the LDAPServers table in the database.
    /// </summary>
    public class LDAPServer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the LDAP server.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the LDAP server.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? Server { get; set; }

        /// <summary>
        /// Gets or sets the port number for the LDAP server.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Port { get; set; }

        /// <summary>
        /// Gets or sets the search base for LDAP queries.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? SearchBase { get; set; }

        /// <summary>
        /// Gets or sets the admin search base for LDAP queries.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? AdminSearchBase { get; set; }
    }
}