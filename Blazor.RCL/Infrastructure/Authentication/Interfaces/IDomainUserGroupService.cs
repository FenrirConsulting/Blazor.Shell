using System.Security.Claims;

namespace Blazor.RCL.Infrastructure.Authentication.Interfaces
{
    /// <summary>
    /// Defines the contract for services that handle domain user groups and roles.
    /// </summary>
    public interface IDomainUserGroupService
    {
        /// <summary>
        /// Retrieves the user groups for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>A list of user group names.</returns>
        List<string> GetUserGroups(ClaimsPrincipal principal);

        /// <summary>
        /// Retrieves the roles for the given identity based on their groups.
        /// </summary>
        /// <param name="identity">The ClaimsPrincipal representing the user.</param>
        /// <returns>A list of role names.</returns>
        List<string> GetRolesFromGroups(ClaimsPrincipal identity);

        /// <summary>
        /// Creates a list of claims for the given username, roles, and authentication time.
        /// </summary>
        /// <param name="username">The username of the authenticated user.</param>
        /// <param name="roles">The list of roles assigned to the user.</param>
        /// <param name="lastAuthenticated">The timestamp of the last authentication.</param>
        /// <returns>A list of claims.</returns>
        List<Claim> CreateClaims(string username, List<string> roles, DateTime lastAuthenticated);
    }
}