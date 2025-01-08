using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Infrastructure.Themes;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Layouts
{
    /// <summary>
    /// Represents the main layout component of the application.
    /// </summary>
    public partial class MainLayout : IDisposable
    {
        #region Injected Services
        /// <summary>
        /// Service for managing the application theme.
        /// </summary>
        [Inject] private IThemeService _themeService { get; set; } = default!;

        /// <summary>
        /// Service for logging.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        #region Private Fields
        private bool _sideBarOpen = true;
        private bool _isLoading = true; // Loading flag
        #endregion

        #region Private Methods
        /// <summary>
        /// Toggles the sidebar open/closed state.
        /// </summary>
        private void ToggleSidebar() => _sideBarOpen = !_sideBarOpen;
        private bool _disposed = false;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and sets up the theme.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await _themeService.InitializeThemeAsync();
                _themeService.OnChange += StateHasChanged;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing MainLayout", ex);
            }
        }

        /// <summary>
        /// Performs actions after the component has rendered.
        /// </summary>
        /// <param name="firstRender">Indicates if this is the first time the component has been rendered.</param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isLoading = false; // Set loading flag to false after initialization
                StateHasChanged(); // Trigger re-render after initialization
            }
            await Task.CompletedTask; // Add an await statement to satisfy the async method requirement
        }

        /// <summary>
        /// Disposes the resources used by the MainLayout component.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _themeService.OnChange -= StateHasChanged;
            }
        }
        #endregion
    }
}