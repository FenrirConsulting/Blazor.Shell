using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Pages
{
    /// <summary>
    /// Represents the Counter page component.
    /// </summary>
    public partial class Counter : ComponentBase
    {
        #region Injected Services
        /// <summary>
        /// Service for handling navigation.
        /// </summary>
        [Inject] private NavigationManager? _navigationManager { get; set; }

        /// <summary>
        /// Service for managing authentication state.
        /// </summary>
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }

        /// <summary>
        /// Service for logging.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        #region Private Fields
        /// <summary>
        /// The current count value.
        /// </summary>
        private int _currentCount = 0;
        #endregion

        #region Private Methods
        /// <summary>
        /// Increments the counter value.
        /// </summary>
        private void IncrementCount()
        {
            _currentCount++;
        }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and checks user authentication and authorization.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await _authenticationStateProvider!.GetAuthenticationStateAsync();
                var user = authState.User;

                if (!user.Identity!.IsAuthenticated)
                {
                    // User is not authenticated, redirect to login page
                    _navigationManager!.NavigateTo("/");
                }
                else if (!user.IsInRole("User"))
                {
                    // User is authenticated but does not have the required role, redirect to an access denied page
                    _navigationManager!.NavigateTo("/account/accessdenied");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred during Counter initialization", ex);
                _navigationManager!.NavigateTo("/error");
            }
        }
        #endregion
    }
}