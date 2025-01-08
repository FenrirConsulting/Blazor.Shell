using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents the navigation menu component of the application.
    /// </summary>
    public partial class NavMenu : ComponentBase, IDisposable
    {
        #region Parameters
        /// <summary>
        /// Gets or sets whether the sidebar is open.
        /// </summary>
        [Parameter] public bool SideBarOpen { get; set; }
        #endregion

        #region Injected Services
        [Inject] protected NavLinksInfoList? Links { get; set; }
        [Inject] protected IAppConfiguration? AppConfiguration { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected AuthenticationStateProvider? _AuthenticationStateProvider { get; set; }
        [Inject] protected IRequestRefresh? _RequestRefresh { get; set; }
        [Inject] protected IThemeService? _ThemeService { get; set; }
        [Inject] protected ILogHelper? _Logger { get; set; }
        #endregion

        #region Private Properties
        private Task<AuthenticationState>? _authenticationState { get; set; }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and sets up event subscriptions.
        /// </summary>
        protected override void OnInitialized()
        {
            try
            {
                _RequestRefresh!.RefreshRequested += RefreshComponent;
                _ThemeService!.OnChange += StateHasChanged;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _Logger?.LogError("Error occurred while initializing NavMenu", ex);
            }
        }

        /// <summary>
        /// CLeans up resources and unsubscribes from events when the component is disposed.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_RequestRefresh != null)
                {
                    _RequestRefresh.RefreshRequested -= RefreshComponent;
                }
            }
            catch( Exception ex)
            {
                _Logger!.LogError("Error occurred while disposing NavMenu", ex);
            }
        }
        #endregion

        #region Private Methods
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

        /// <summary>
        /// Logs out the current user and navigates to the logout page.
        /// </summary>
        private void Logout()
        {
            try
            {
                NavigationManager!.NavigateTo("/account/logout");
            }
            catch (Exception ex)
            {
                _Logger?.LogError("Error occurred during logout", ex);
            }
        }
        #endregion
    }
}