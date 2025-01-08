using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Application.Models
{
    /// <summary>
    /// Represents information for a navigation link.
    /// </summary>
    public class NavLinkInfo
    {
        /// <summary>
        /// Gets or sets the icon for the navigation link.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Gets or sets the icon type for the navigation link.
        /// </summary>
        public string? IconType { get; set; }

        /// <summary>
        /// Gets or sets the URL for the navigation link.
        /// </summary>
        [Url(ErrorMessage = "Invalid URL format.")]
        public string? Href { get; set; }

        /// <summary>
        /// Gets or sets the display text for the navigation link.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets the role required to access this navigation link.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Gets or sets the position of the navigation link.
        /// </summary>
        public string? Position { get; set; }
    }
}