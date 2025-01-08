using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents the application bar component.
    /// </summary>
    public partial class AppBar : ComponentBase, IDisposable
    {
        #region Parameters and Injected Services
        /// <summary>
        /// Event callback for toggling the sidebar.
        /// </summary>
        [Parameter] public EventCallback OnSidebarToggled { get; set; }

        /// <summary>
        /// Application configuration service.
        /// </summary>
        [Inject] private IAppConfiguration _appConfiguration { get; set; } = default!;

        /// <summary>
        /// Authentication state provider.
        /// </summary>
        [Inject] private AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;

        /// <summary>
        /// Navigation manager for handling routing.
        /// </summary>
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        /// <summary>
        /// Theme service for managing application theme.
        /// </summary>
        [Inject] private IThemeService _themeService { get; set; } = default!;

        /// <summary>
        /// Logging service.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        #region Private Fields
        /// <summary>
        /// List of user roles.
        /// </summary>
        private List<string> _userRoles = new List<string>();

        /// <summary>
        /// User's display name.
        /// </summary>
        private string _userDisplayName = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the user's display name.
        /// </summary>
        public string UserDisplayName => _userDisplayName;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and loads user information.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    _userDisplayName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                    _userRoles = user.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing AppBar", ex);
            }
        }

        /// <summary>
        /// Disposes the resources used by the AppBar component.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Currently, there are no resources to dispose.
                // This method is implemented for future-proofing and consistency.
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while disposing AppBar", ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Toggles the application theme between light and dark mode.
        /// </summary>
        private async Task ToggleTheme()
        {
            try
            {
                await _themeService.ToggleThemeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while toggling theme", ex);
            }
        }

        /// <summary>
        /// Logs out the current user and navigates to the logout page.
        /// </summary>
        private void Logout()
        {
            try
            {
                _navigationManager.NavigateTo($"{_appConfiguration.BaseUrl}Account/Logout", forceLoad: true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred during logout", ex);
            }
        }
        #endregion
    }
}