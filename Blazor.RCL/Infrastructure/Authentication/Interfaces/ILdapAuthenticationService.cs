namespace Blazor.RCL.Infrastructure.Authentication.Interfaces
{
    /// <summary>
    /// Defines the contract for LDAP authentication services.
    /// </summary>
    public interface ILdapAuthenticationService
    {
        /// <summary>
        /// Validates user credentials against the LDAP server.
        /// </summary>
        /// <param name="ldapServer">The LDAP server address.</param>
        /// <param name="port">The LDAP server port.</param>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>A tuple containing authentication status and a message.</returns>
        Task<(bool isAuthenticated, string message)> ValidateCredentialsAsync(
            string ldapServer, int port, string username, string password);

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
        /// <returns>A tuple containing authentication status and user groups. If authentication fails, the groups list will be empty.</returns>
        Task<(bool isAuthenticated, List<string> groups)> ValidateCredentialsAndFetchGroupsAsync(
            string ldapName, string ldapServer, string ldapPort, string searchBase, string adminSearchBase, string username, string password);

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
        Task<(bool isAuthenticated, List<string> roles)> ValidateCredentialsAndFetchRolesAsync(
            string ldapName, string ldapServer, string ldapPort, string searchBase, string adminSearchBase, string username, string password);
    }
}