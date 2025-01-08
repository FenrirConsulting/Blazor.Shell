using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities
{
    /// <summary>
    /// Represents a remote script configuration in the application.
    /// This entity defines the structure for the RemoteScripts table in the database.
    /// </summary>
    public class RemoteScript
    {
        /// <summary>
        /// Gets or sets the unique identifier for the remote script.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the location of the remote script.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the remote script.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the variables associated with the remote script.
        /// </summary>
        public string? Variables { get; set; }

        /// <summary>
        /// Gets or sets the user who added the record.
        /// </summary>
        public string AuditAddBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was added.
        /// </summary>
        public DateTime AuditAddDate { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the record.
        /// </summary>
        public string? AuditChangeBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was last modified.
        /// </summary>
        public DateTime? AuditChangeDate { get; set; }
    }
}