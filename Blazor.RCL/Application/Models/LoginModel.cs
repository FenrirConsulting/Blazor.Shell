using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Application.Models
{
    /// <summary>
    /// Represents the login information for user authentication.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Gets or sets the selected server for authentication.
        /// </summary>
        [Required(ErrorMessage = "Server selection is required.")]
        public string SelectedServer { get; set; } = "";

        /// <summary>
        /// Gets or sets the username for authentication.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = "";

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        /// <summary>
        /// Gets or sets the SAM Account Name.
        /// </summary>
        public string SAMAccountName { get; set; } = "";
    }
}