using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Configuration
{
    /// <summary>
    /// Represents user settings in the application.
    /// This entity defines the structure for the UserSettings table in the database.
    /// </summary>
    public class UserSettings
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the user settings.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Username associated with these settings.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string? Username { get; set; }

        /// <summary>
        /// Indicates whether dark mode is enabled for the user.
        /// </summary>
        [MaxLength(10)]
        public string? DarkMode { get; set; }

        /// <summary>
        /// Stores additional user-specific settings as a string (consider using JSON for complex settings).
        /// </summary>
        public string? AdditionalSettings { get; set; }

        #endregion
    }
}