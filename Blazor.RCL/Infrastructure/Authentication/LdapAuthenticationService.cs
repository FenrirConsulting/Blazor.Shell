using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Utilclass;

namespace Blazor.RCL.Infrastructure.Authentication
{
    /// <summary>
    /// Provides LDAP authentication services, including credential validation and role fetching.
    /// </summary>
    public class LdapAuthenticationService : ILdapAuthenticationService
    {
        #region Private Fields

        private readonly ILogHelper _logHelper;
        private readonly LdapRoleMappingConfig _ldapRoleMappingConfig;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the LdapAuthenticationService class.
        /// </summary>
        /// <param name="ldapRoleMappingConfig">Configuration for LDAP role mappings.</param>
        /// <param name="logHelper">Helper for logging errors and messages.</param>
        public LdapAuthenticationService(LdapRoleMappingConfig ldapRoleMappingConfig, ILogHelper logHelper)
        {
            _ldapRoleMappingConfig = ldapRoleMappingConfig ?? throw new ArgumentNullException(nameof(ldapRoleMappingConfig));
            _logHelper = logHelper ?? throw new ArgumentNullException(nameof(logHelper));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates user credentials against the LDAP server.
        /// </summary>
        /// <param name="ldapServer">The LDAP server address.</param>
        /// <param name="port">The LDAP server port.</param>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>A tuple containing authentication status and a message.</returns>
        public async Task<(bool isAuthenticated, string message)> ValidateCredentialsAsync(
            string ldapServer, int port, string username, string password)
        {
            string dn = $"{username}@{ldapServer}";

            try
            {
                using (var connection = new LdapConnection())
                {
                    await Task.Run(() =>
                    {
                        connection.Connect(ldapServer, port);
                        connection.Bind(dn, password);
                    });

                    if (connection.Bound)
                    {
                        return (true, "Authentication successful");
                    }
                    else
                    {
                        return (false, "Authentication failed");
                    }
                }
            }
            catch (LdapException ex)
            {
                _logHelper.LogError($"LDAP Exception occurred: {ex.Message}", ex);
                return (false, $"LDAP error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logHelper.LogError($"An unexpected error occurred: {ex.Message}", ex);
                return (false, $"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates user credentials and fetches groups from the LDAP server.
        /// </summary>
        /// <param name="ldapName">The name of the LDAP server.</param>
        /// <param name="ldapServer">The LDAP server address.</param>
        /// <param name="ldapPort">The LDAP server port.</param>
        /// <param name="searchBase">The search base for LDAP queries.</param>
        /// <param name="adminSearchBase">The admin search base for LDAP queries.</param>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>A tuple containing authentication status and user groups.</returns>
        public async Task<(bool isAuthenticated, List<string> groups)> ValidateCredentialsAndFetchGroupsAsync(
            string ldapName, string ldapServer, string ldapPort, string searchBase, string adminSearchBase, string username, string password)
        {
            var groups = new List<string>();
            string dn = string.Format(@"{0}@{1}", username, ldapServer);

            try
            {
                using (var connection = new LdapConnection())
                {
                    await ConnectAndBindToLdapServerAsync(connection, ldapServer, ldapPort, dn, password);

                    _logHelper.LogMessage($"Connected to LDAP server {ldapName} as {username}");

                    if (connection.Bound)
                    {
                        await SearchAndCollectGroupsAsync(connection, searchBase, adminSearchBase, username, groups);
                        return (true, groups);
                    }
                }
            }
            catch (LdapException ex)
            {
                _logHelper.LogError($"LDAP Exception occurred while fetching groups: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logHelper.LogError($"An unexpected error occurred while fetching groups: {ex.Message}", ex);
            }

            // If we've reached this point, an error occurred during group fetching
            return (false, new List<string>());
        }

        /// <summary>
        /// Validates user credentials and fetches roles from the LDAP server.
        /// </summary>
        /// <param name="ldapName">The name of the LDAP server.</param>
        /// <param name="ldapServer">The LDAP server address.</param>
        /// <param name="ldapPort">The LDAP server port.</param>
        /// <param name="searchBase">The search base for LDAP queries.</param>
        /// <param name="adminSearchBase">The admin search base for LDAP queries.</param>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>A tuple containing authentication status and user roles.</returns>
        public async Task<(bool isAuthenticated, List<string> roles)> ValidateCredentialsAndFetchRolesAsync(
            string ldapName, string ldapServer, string ldapPort, string searchBase, string adminSearchBase, string username, string password)
        {
            var groups = new List<string>();
            string dn = string.Format(@"{0}@{1}", username, ldapServer);

            try
            {
                using (var connection = new LdapConnection())
                {
                    await ConnectAndBindToLdapServerAsync(connection, ldapServer, ldapPort, dn, password);

                    if (connection.Bound)
                    {
                        await SearchAndCollectGroupsAsync(connection, searchBase, adminSearchBase, username, groups);
                        var roles = MapGroupsToRoles(groups);
                        return (true, roles);
                    }
                }
            }
            catch (LdapException ex)
            {
                _logHelper.LogError($"LDAP Exception occurred: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logHelper.LogError($"An unexpected error occurred: {ex.Message}", ex);
            }

            return (false, new List<string>());
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Connects and binds to the LDAP server.
        /// </summary>
        private async Task ConnectAndBindToLdapServerAsync(LdapConnection connection, string ldapServer, string ldapPort, string dn, string password)
        {
            await Task.Run(() =>
            {
                try
                {
                    int port;
                    if (int.TryParse(ldapPort, out port))
                    {
                        connection.Connect(ldapServer, port);
                        connection.Bind(dn, password);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid LDAP port.");
                    }
                }
                catch (LdapException ex)
                {
                    _logHelper.LogError($"Error connecting to LDAP server: {ex.Message}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// Searches the LDAP server and collects user groups.
        /// </summary>
        private async Task SearchAndCollectGroupsAsync(LdapConnection connection, string searchBase, string adminSearchBase, string username, List<string> groups)
        {
            var searchFilter = $"(&(objectClass=user)(sAMAccountName={username}))";
            var searchScope = LdapConnection.ScopeSub;
            var searchConstraints = new LdapSearchConstraints
            {
                MaxResults = 1000,
                TimeLimit = 10000,
                ReferralFollowing = true
            };

            await Task.Run(() =>
            {
                try
                {
                    CollectGroupsFromSearchResults(connection.Search(searchBase, searchScope, searchFilter, null, false, searchConstraints), groups);
                    CollectGroupsFromSearchResults(connection.Search(adminSearchBase, searchScope, searchFilter, null, false, searchConstraints), groups);
                }
                catch (LdapException ex)
                {
                    _logHelper.LogError($"Error searching LDAP: {ex.Message}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// Collects groups from LDAP search results.
        /// </summary>
        private void CollectGroupsFromSearchResults(ILdapSearchResults searchResults, List<string> groups)
        {
            while (searchResults.HasMore())
            {
                var entry = searchResults.Next();
                var attribute = entry.GetAttribute("memberOf");

                if (attribute != null)
                {
                    foreach (var value in attribute.StringValueArray)
                    {
                        var cnPart = value.Split(',').FirstOrDefault(part => part.Trim().StartsWith("CN="));
                        if (cnPart != null)
                        {
                            var groupName = cnPart.Substring(3);
                            groups.Add(groupName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Maps LDAP groups to roles based on configuration.
        /// </summary>
        private List<string> MapGroupsToRoles(List<string> groups)
        {
            return _ldapRoleMappingConfig.LdapRoleMappings
                .Where(roleMapping => roleMapping.Value.Any(mappedGroup => groups.Contains(mappedGroup, StringComparer.OrdinalIgnoreCase)))
                .Select(roleMapping => roleMapping.Key)
                .Distinct()
                .ToList();
        }
        #endregion
    }
}