using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Provides caching functionality for user groups and roles.
    /// </summary>
    public static class UserGroupsCache
    {
        #region Private Fields

        private static readonly Dictionary<string, List<string>> _userGroups = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<string>> _userRoles = new Dictionary<string, List<string>>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the dictionary of user groups.
        /// </summary>
        public static IReadOnlyDictionary<string, List<string>> UserGroups => _userGroups;

        /// <summary>
        /// Gets the dictionary of user roles.
        /// </summary>
        public static IReadOnlyDictionary<string, List<string>> UserRoles => _userRoles;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds or updates the groups for a given user.
        /// </summary>
        /// <param name="userName">The username of the authenticated user.</param>
        /// <param name="groups">The list of groups to add or update.</param>
        public static void AddUserGroups(string userName, List<string> groups)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (groups == null)
                throw new ArgumentNullException(nameof(groups));

            if (!_userGroups.ContainsKey(userName))
            {
                _userGroups[userName] = new List<string>(groups);
            }
            else
            {
                _userGroups[userName] = _userGroups[userName].Union(groups).ToList();
            }
        }

        /// <summary>
        /// Retrieves the groups for a given user.
        /// </summary>
        /// <param name="userName">The username of the authenticated user.</param>
        /// <returns>A list of groups the user belongs to.</returns>
        public static List<string> GetUserGroups(string userName)
        {
            return _userGroups.TryGetValue(userName, out var groups) ? new List<string>(groups) : new List<string>();
        }

        /// <summary>
        /// Adds or updates the roles for a given user.
        /// </summary>
        /// <param name="userName">The username of the authenticated user.</param>
        /// <param name="roles">The list of roles to add or update.</param>
        public static void AddUserRoles(string userName, List<string> roles)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            if (!_userRoles.ContainsKey(userName))
            {
                _userRoles[userName] = new List<string>(roles);
            }
            else
            {
                _userRoles[userName] = _userRoles[userName].Union(roles).ToList();
            }
        }

        /// <summary>
        /// Retrieves the roles for a given user.
        /// </summary>
        /// <param name="userName">The username of the authenticated user.</param>
        /// <returns>A list of roles the user belongs to.</returns>
        public static List<string> GetUserRoles(string userName)
        {
            return _userRoles.TryGetValue(userName, out var roles) ? new List<string>(roles) : new List<string>();
        }

        #endregion
    }
}