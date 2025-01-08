using System.Linq;
using System.Security.Claims;

namespace Blazor.RCL.Infrastructure.Authorization
{
    /// <summary>
    /// Provides extension methods for ClaimsPrincipal to enhance authorization capabilities.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the user has any role claims.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the user.</param>
        /// <returns>True if the user has at least one role claim, otherwise false.</returns>
        public static bool HasAnyRole(this ClaimsPrincipal user)
        {
            return user.Claims.Any(c => c.Type == ClaimTypes.Role);
        }

        #endregion
    }
}