using System;
using System.Collections.Generic;

namespace Blazor.RCL.Application.Models
{
    /// <summary>
    /// Represents the response model for a login attempt.
    /// </summary>
    public class LoginResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the username of the authenticated user.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the list of roles assigned to the authenticated user.
        /// </summary>
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last successful authentication.
        /// </summary>
        public DateTime LastAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the expiration date and time of the authentication session.
        /// </summary>
        public DateTime? ExpiresUtc { get; set; }

        #endregion
    }
}