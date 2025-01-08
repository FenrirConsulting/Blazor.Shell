using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Configuration
{
    /// <summary>
    /// Represents configuration settings for various tools in the application.
    /// This entity defines the structure for the ToolsConfiguration table in the database.
    /// </summary>
    public class ToolsConfiguration
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the configuration entry.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// The name or key of the configuration setting.
        /// </summary>
        [Required]
        public string? Setting { get; set; }

        /// <summary>
        /// The name of the application this configuration applies to.
        /// </summary>
        [Required]
        public string? Application { get; set; }

        /// <summary>
        /// The value of the configuration setting.
        /// </summary>
        [Required]
        public string? Value { get; set; }

        #endregion
    }
}