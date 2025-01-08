using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities
{
    /// <summary>
    /// Represents a service account configuration in the application.
    /// This entity defines the structure for the ServiceAccounts table in the database.
    /// </summary>
    public class ServiceAccount
    {
        /// <summary>
        /// Gets or sets the unique identifier for the service account.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the SAM (Security Account Manager) name for the service account.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? SAM { get; set; }

        /// <summary>
        /// Gets or sets the domain of the service account.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the key type for the service account.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? KeyType { get; set; }

        /// <summary>
        /// Gets or sets the key path for the service account.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? KeyPath { get; set; }
    }
}