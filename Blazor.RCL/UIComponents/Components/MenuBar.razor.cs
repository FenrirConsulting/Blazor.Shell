using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Infrastructure.Services;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents the menu bar component of the application.
    /// </summary>
    public partial class MenuBar : ComponentBase, IDisposable
    {
        #region Injected Services
        [Inject] protected IThemeService _ThemeService { get; set; } = default!;
        [Inject] protected ILogHelper _Logger { get; set; } = default!;
        [Inject] protected IRequestRefresh? _RequestRefresh { get; set; }
        [Inject] protected IAppConfiguration _AppConfiguration { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider _AuthenticationStateProvider { get; set; } = default!;
        [Inject] protected NavigationManager _NavigationManager { get; set; } = default!;
        [Inject] protected NavLinksInfoList _Links { get; set; } = default!;
        #endregion

        #region Protected Properties
        protected List<string> UserRoles = new List<string>();
        protected string UserDisplayName = string.Empty;
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines if the given href matches the current page.
        /// </summary>
        /// <param name="href">The href to check.</param>
        /// <returns>True if the current page matches the href, false otherwise.</returns>
        private bool IsActive(string href)
        {
            // Implement logic to determine if the current page matches the href
            // This is a placeholder and needs to be implemented based on your routing logic
            return _NavigationManager.Uri.EndsWith(href);
        }

        /// <summary>
        /// Refreshes the component when requested.
        /// </summary>
        private void RefreshComponent()
        {
            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _Logger?.LogError("Error occurred while refreshing NavMenu", ex);
            }
        }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and loads user information.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await _AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    UserDisplayName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                    UserRoles = user.Claims
                                    .Where(c => c.Type == ClaimTypes.Role)
                                    .Select(c => c.Value)
                    .ToList();
                }

                _RequestRefresh!.RefreshRequested += RefreshComponent;
                _ThemeService.OnChange += StateHasChanged;
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while initializing MenuBar", ex);
            }
        }

        /// <summary>
        /// Disposes the resources used by the MenuBar component.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _ThemeService.OnChange -= StateHasChanged;
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while disposing MenuBar", ex);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Toggles the application theme between light and dark mode.
        /// </summary>
        protected async Task ToggleTheme()
        {
            try
            {
                await _ThemeService.ToggleThemeAsync();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while toggling theme", ex);
            }
        }

        /// <summary>
        /// Logs out the current user and navigates to the logout page.
        /// </summary>
        protected void Logout()
        {
            try
            {
                _NavigationManager.NavigateTo($"{_AppConfiguration.BaseUrl}account/logout", forceLoad: true);
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred during logout", ex);
            }
        }
        #endregion
    }
}