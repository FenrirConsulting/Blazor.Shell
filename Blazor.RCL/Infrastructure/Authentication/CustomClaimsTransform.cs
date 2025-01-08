using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;

namespace Blazor.RCL.Infrastructure.Authentication
{
    /// <summary>
    /// Provides custom claims transformation functionality for authenticated users.
    /// </summary>
    public class CustomClaimsTransform : IClaimsTransformation
    {
        #region Private Fields

        private readonly ILogHelper _logHelper;
        private readonly IDistributedCache _cache;
        private readonly IAppConfiguration _appConfiguration;
        private readonly IUserSettingsService _userSettingsService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomClaimsTransform class.
        /// </summary>
        /// <param name="logHelper">The logging helper for error and message logging.</param>
        /// <param name="cache">The distributed cache for storing and retrieving user roles.</param>
        /// <param name="appConfiguration">The application configuration service.</param>
        /// <param name="userSettingsService">The user settings service.</param>
        public CustomClaimsTransform(
            ILogHelper logHelper,
            IDistributedCache cache,
            IAppConfiguration appConfiguration,
            IUserSettingsService userSettingsService)
        {
            _logHelper = logHelper ?? throw new ArgumentNullException(nameof(logHelper));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Transforms the claims principal by adding cached roles to the existing claims.
        /// </summary>
        /// <param name="principal">The claims principal to transform.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the transformed claims principal.</returns>
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity?.IsAuthenticated ?? true)
            {
                return principal;
            }

            string username = principal.Identity!.Name!;
            try
            {
                var cachedRoles = await GetCachedRolesAsync(username);
                var currentRoles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet();
                var allRoles = currentRoles.Union(cachedRoles).ToHashSet();

                var newIdentity = new ClaimsIdentity(principal.Identity);
                UpdateRoleClaims(newIdentity, allRoles);

                return new ClaimsPrincipal(newIdentity);
            }
            catch (Exception ex)
            {
                _logHelper.LogError($"Error transforming claims for user {username}: {ex.Message}", ex);
                return principal;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves cached roles for a given username.
        /// </summary>
        /// <param name="username">The username to retrieve roles for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a HashSet of role names.</returns>
        private async Task<HashSet<string>> GetCachedRolesAsync(string username)
        {
            try
            {
                byte[]? cachedRolesBytes = await _cache.GetAsync($"user_roles_{username}");
                if (cachedRolesBytes != null)
                {
                    string serializedRoles = Encoding.UTF8.GetString(cachedRolesBytes);
                    return JsonSerializer.Deserialize<HashSet<string>>(serializedRoles) ?? new HashSet<string>();
                }
                return new HashSet<string>();
            }
            catch (Exception ex)
            {
                _logHelper.LogError($"Error retrieving cached roles for user {username}: {ex.Message}", ex);
                return new HashSet<string>();
            }
        }

        /// <summary>
        /// Updates the role claims in the given identity.
        /// </summary>
        /// <param name="identity">The claims identity to update.</param>
        /// <param name="roles">The set of roles to add to the identity.</param>
        private void UpdateRoleClaims(ClaimsIdentity identity, HashSet<string> roles)
        {
            try
            {
                var existingRoleClaims = identity.FindAll(ClaimTypes.Role).ToList();
                foreach (var claim in existingRoleClaims)
                {
                    identity.RemoveClaim(claim);
                }

                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
            catch (Exception ex)
            {
                _logHelper.LogError($"Error updating role claims: {ex.Message}", ex);
            }
        }

        #endregion
    }
}