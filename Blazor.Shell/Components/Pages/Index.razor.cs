using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.Shell.Components.Pages
{
    /// <summary>
    /// Represents the index page component for the ApplicationShell.
    /// </summary>
    public partial class Index : ComponentBase
    {
        #region Injected Services
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] private IDomainUserGroupService? _domainUserGroupService { get; set; }
        [Inject] private IUserSettingsRepository? _userSettingsRepository { get; set; }
        [Inject] private ILogHelper? _logger { get; set; }
        [Inject] private LdapRoleMappingConfig? _ldapRoleMappingConfig { get; set; }
        #endregion

        #region Private Fields
        private string? _databaseDisplayName;
        private string? _userDisplayName;
        private List<string>? _userGroups;
        private List<string>? _userRoles;
        private List<string>? _ldapRoleMappings;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and retrieves user authentication information.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadUserInformation();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads user information from the authentication state and related services.
        /// </summary>
        private async Task LoadUserInformation()
        {
            var authenticationState = await _authenticationStateProvider!.GetAuthenticationStateAsync();
            var user = authenticationState.User;

            if (user.Identity!.IsAuthenticated)
            {
                try
                {
                    _userDisplayName = user.Identity.Name;
                    _userGroups = UserGroupsCache.GetUserGroups(_userDisplayName!);

                    _userRoles = user.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

                    _ldapRoleMappings = _ldapRoleMappingConfig!.LdapRoleMappings.Keys.ToList();

                    var userSettings = await _userSettingsRepository!.GetUserSettingsAsync(_userDisplayName!);
                    _databaseDisplayName = userSettings?.Username;
                }
                catch (Exception ex)
                {
                    _logger!.LogError("Error getting user settings: {0}", ex);
                }
            }
        }
        #endregion
    }
}