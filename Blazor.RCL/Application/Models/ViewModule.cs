using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Application.Models
{
    /// <summary>
    /// Represents a view module with its associated URLs.
    /// </summary>
    public class ViewModule
    {
        /// <summary>
        /// Gets or sets the URL for the view module.
        /// </summary>
        [Required(ErrorMessage = "URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the development URI for the view module.
        /// </summary>
        [Url(ErrorMessage = "Invalid URL format.")]
        public string DevelopmentUri { get; set; } = string.Empty;
    }
}