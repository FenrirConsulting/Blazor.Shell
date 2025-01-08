using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Blazor.Shell
{
    /// <summary>
    /// Represents the main application component for the AD Lookup tool.
    /// </summary>
    public partial class App
    {
        #region Injected Services
        /// <summary>
        /// The application configuration service.
        /// </summary>
        [Inject] public IAppConfiguration? AppConfiguration { get; set; }
        /// <summary>
        /// Service for requesting UI refresh.
        /// </summary>
        [Inject] IRequestRefresh? _RequestRefresh { get; set; }
        /// <summary>
        /// Service for managing application theme.
        /// </summary>
        [Inject] private IThemeService? _ThemeService { get; set; }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and sets up event handlers.
        /// </summary>
        protected override void OnInitialized()
        {
            _RequestRefresh!.RefreshRequested += RefreshRequested;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the refresh request and updates the component's state.
        /// </summary>
        public async void RefreshRequested()
        {
            await InvokeAsync(StateHasChanged);
        }
        #endregion
    }
}
