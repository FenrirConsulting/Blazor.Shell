using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Authentication
{
    /// <summary>
    /// Provides services for retrieving user groups and roles from Active Directory.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class DomainUserGroupService : IDomainUserGroupService
    {
        #region Private Fields

        private readonly ILogHelper _logger;
        private readonly LdapRoleMappingConfig _ldapRoleMappingConfig;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DomainUserGroupService class.
        /// </summary>
        /// <param name="logger">The logger for error and message logging.</param>
        /// <param name="ldapRoleMappingConfig">The configuration for LDAP role mappings.</param>
        public DomainUserGroupService(ILogHelper logger, LdapRoleMappingConfig ldapRoleMappingConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ldapRoleMappingConfig = ldapRoleMappingConfig ?? throw new ArgumentNullException(nameof(ldapRoleMappingConfig));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the user groups for the given principal from Active Directory.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>A list of user group names.</returns>
        public List<string> GetUserGroups(ClaimsPrincipal principal)
        {
            var groups = new List<string>();
            try
            {
                var principalIdentity = principal.Identity;
                if (principalIdentity == null || string.IsNullOrEmpty(principalIdentity.Name))
                {
                    _logger.LogWarn("Principal identity is null or empty");
                    return groups;
                }

                using (var context = new PrincipalContext(ContextType.Domain))
                {

                    using (var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, principalIdentity.Name))
                    {
                        if (userPrincipal == null)
                        {
                            return groups;
                        }

                        var userGroups = userPrincipal.GetGroups();

                        foreach (var group in userGroups)
                        {
                            try
                            {
                                groups.Add(group.SamAccountName);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error processing group for user {principalIdentity.Name}: {ex.Message}", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error occurred while retrieving user groups: {ex.Message}", ex);
            }

            return groups;
        }

        /// <summary>
        /// Retrieves the roles for the given identity based on their Active Directory groups.
        /// </summary>
        /// <param name="identity">The ClaimsPrincipal representing the user.</param>
        /// <returns>A list of role names.</returns>
        public List<string> GetRolesFromGroups(ClaimsPrincipal identity)
        {
            try
            {
                var userGroups = GetUserGroups(identity);
                return _ldapRoleMappingConfig.LdapRoleMappings
                    .Where(roleMapping => roleMapping.Value.Any(mappedGroup => userGroups.Contains(mappedGroup, StringComparer.OrdinalIgnoreCase)))
                    .Select(roleMapping => roleMapping.Key)
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while retrieving roles from groups.", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// Creates a list of claims for the given username, roles, and authentication time.
        /// </summary>
        /// <param name="username">The username of the authenticated user.</param>
        /// <param name="roles">The list of roles assigned to the user.</param>
        /// <param name="lastAuthenticated">The timestamp of the last authentication.</param>
        /// <returns>A list of claims.</returns>
        public List<Claim> CreateClaims(string username, List<string> roles, DateTime lastAuthenticated)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim("AuthenticationTimestamp", lastAuthenticated.ToString()),
                    new Claim(ClaimTypes.Name, username)
                };
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while creating claims.", ex);
                return new List<Claim>();
            }
        }

        #endregion
    }
}