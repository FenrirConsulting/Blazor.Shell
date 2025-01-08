using System;
using System.Collections.Generic;

namespace Blazor.RCL.Application.Models
{
    /// <summary>
    /// Represents the response model containing user information.
    /// </summary>
    public class UserInfoResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the list of claims associated with the user.
        /// </summary>
        public List<string>? Claims { get; set; }

        /// <summary>
        /// Gets or sets the user settings as a string (consider using a more structured format if needed).
        /// </summary>
        public string? Settings { get; set; }

        /// <summary>
        /// Gets or sets the list of roles assigned to the user.
        /// </summary>
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last successful authentication.
        /// </summary>
        public DateTime LastAuthenticated { get; set; }

        #endregion
    }
}