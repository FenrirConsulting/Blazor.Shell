using System.Collections.Generic;

namespace Blazor.RCL.Application.Models
{
    /// <summary>
    /// Represents a request to update the roles and groups of a user.
    /// </summary>
    public class RoleUpdateRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the username of the user whose roles are being updated.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the list of groups to be assigned to the user.
        /// </summary>
        public List<string>? Groups { get; set; }

        /// <summary>
        /// Gets or sets the list of roles to be assigned to the user.
        /// </summary>
        public List<string>? Roles { get; set; }

        #endregion
    }
}