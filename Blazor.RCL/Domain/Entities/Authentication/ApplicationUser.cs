using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Blazor.RCL.Domain.Entities.Authentication
{
    /// <summary>
    /// Represents an application user, extending the IdentityUser class.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the user's unique identifier.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        public override string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public override string? Email { get; set; }

        /// <summary>
        /// Gets or sets the list of claims associated with the user.
        /// </summary>
        public List<string>? Claims { get; set; }

        /// <summary>
        /// Gets or sets the user's settings as a JSON string.
        /// </summary>
        public string? Settings { get; set; }

        /// <summary>
        /// Gets or sets the list of roles associated with the user.
        /// </summary>
        public List<string>? Roles { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ApplicationUser class.
        /// </summary>
        public ApplicationUser() : base()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        #endregion
    }
}