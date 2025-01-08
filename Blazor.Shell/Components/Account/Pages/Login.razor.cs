using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using MudBlazor;
using System.Text;
using System.Text.Json;
using Blazor.RCL.Application.Models;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.Shell.Components.Account.Pages
{
    /// <summary>
    /// Represents the login page component.
    /// </summary>
    public partial class Login : ComponentBase
    {
        #region Injected Services
        [Inject] private ILdapAuthenticationService? _ldapAuthenticationService { get; set; }
        [Inject] private LdapServerList? _ldapServerList { get; set; }
        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] private IClaimsTransformation? _claimsTransform { get; set; }
        [Inject] private IRequestRefresh? _requestRefresh { get; set; }
        [Inject] private ILogHelper? _logger { get; set; }
        [Inject] private RoleUpdateRequest? _roleUpdateRequest { get; set; }
        [Inject] private IHttpClientFactory? _httpClientFactory { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private IDistributedCache? _cache { get; set; }
        #endregion

        #region Private Fields
        private MudForm? _form;
        private bool _showErrorMessage;
        private string _errorMessage = string.Empty;
        private bool _showSuccessMessage;
        private string _successMessage = string.Empty;
        private bool _isLoading;
        private string _currentUserName = string.Empty;
        #endregion

        #region Properties
        [SupplyParameterFromForm]
        private LoginModel LoginModel { get; set; } = new LoginModel();
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and retrieves the current user's authentication state.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            var authState = await _authenticationStateProvider!.GetAuthenticationStateAsync();
            _currentUserName = authState.User.Identity?.Name ?? "Anonymous";
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the Enter key press event on the login form.
        /// </summary>
        private async Task HandleEnterKeyPress(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await HandleLogin();
            }
        }

        /// <summary>
        /// Handles the login process, including authentication and role update.
        /// </summary>
        private async Task HandleLogin()
        {
            _isLoading = true;
            var selectedServer = _ldapServerList?.Servers.FirstOrDefault(s => s.Server == LoginModel.SelectedServer);

            if (selectedServer == null || _ldapAuthenticationService == null)
            {
                ShowError("Select a Domain for Login.");
                return;
            }

            try
            {
                var (isAuthenticated, roles) = await AuthenticateUser(selectedServer);

                if (!isAuthenticated)
                {
                    ShowError("Invalid username or password. Please try again.");
                    return;
                }

                await UpdateUserRoles(roles);
                await UpdateLocalCache(roles);

                ShowSuccess("Added Roles: " + string.Join(", ", roles));

                _navigationManager!.NavigateTo(_appConfiguration!.BaseUrl, forceLoad: true);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.Contains("Failed to connect") ? "Invalid Server Name" : ex.Message);
                _logger!.LogError(ex.Message, ex);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task<(bool isAuthenticated, List<string> roles)> AuthenticateUser(LDAPServer selectedServer)
        {
            return await _ldapAuthenticationService!.ValidateCredentialsAndFetchRolesAsync(
                selectedServer.Name!,
                selectedServer.Server!,
                selectedServer.Port!,
                selectedServer.SearchBase!,
                selectedServer.AdminSearchBase!,
                LoginModel.Username,
                LoginModel.Password
            );
        }

        private async Task UpdateUserRoles(List<string> roles)
        {
            _roleUpdateRequest!.Roles = roles;

            var httpClient = _httpClientFactory!.CreateClient(_appConfiguration!.CookieName);
            var jsonContent = JsonSerializer.Serialize(_roleUpdateRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{_appConfiguration.AuthenticationAPIURL}updateroles", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update roles.");
            }
        }

        private async Task UpdateLocalCache(List<string> roles)
        {
            await _cache!.SetStringAsync(
                $"user_roles_{_currentUserName}",
                JsonSerializer.Serialize(roles),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_appConfiguration!.SessionTimeout)
                }
            );
        }

        private void ShowError(string message)
        {
            _showErrorMessage = true;
            _errorMessage = message;
            _isLoading = false;
        }

        private void ShowSuccess(string message)
        {
            _showSuccessMessage = true;
            _successMessage = message;
            _isLoading = false;
        }
        #endregion
    }
}