using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Blazor.RCL.Domain.Entities.Authentication;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Data;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blazor.RCL.Infrastructure.Authentication
{
    /// <summary>
    /// Custom authentication state provider for handling user authentication and authorization.
    /// This class extends RevalidatingServerAuthenticationStateProvider to provide additional
    /// functionality for managing user authentication state, caching, and user settings.
    /// </summary>
    public sealed class CustomAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        #region Private Fields

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PersistentComponentState _state;
        private readonly IdentityOptions _options;
        private readonly PersistingComponentStateSubscription _subscription;
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogHelper _logger;
        private Task<AuthenticationState>? _authenticationStateTask;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the interval at which the authentication state should be revalidated.
        /// </summary>
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomAuthenticationStateProvider class.
        /// </summary>
        /// <param name="persistentComponentState">Provides access to the persistent component state.</param>
        /// <param name="loggerFactory">Factory for creating loggers.</param>
        /// <param name="serviceScopeFactory">Factory for creating service scopes.</param>
        /// <param name="optionsAccessor">Accessor for Identity options.</param>
        /// <param name="cache">Distributed cache for storing user roles.</param>
        /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
        /// <param name="logger">Logger for error and message logging.</param>
        public CustomAuthenticationStateProvider(
            PersistentComponentState persistentComponentState,
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<IdentityOptions> optionsAccessor,
            IDistributedCache cache,
            IHttpContextAccessor httpContextAccessor,
            ILogHelper logger)
            : base(loggerFactory)
        {
            _scopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _state = persistentComponentState ?? throw new ArgumentNullException(nameof(persistentComponentState));
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            AuthenticationStateChanged += OnAuthenticationStateChanged;
            _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the authentication state and notifies listeners of the change.
        /// </summary>
        public async Task RefreshAuthenticationState()
        {
            try
            {
                var authenticationState = await GetAuthenticationStateAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while refreshing authentication state.", ex);
            }
        }

        /// <summary>
        /// Retrieves the user settings for the currently authenticated user.
        /// </summary>
        /// <returns>The user settings for the authenticated user.</returns>
        public async Task<UserSettings> GetUserSettingsAsync()
        {
            try
            {
                var authState = await GetAuthenticationStateAsync();
                using var scope = _scopeFactory.CreateScope();
                var userSettingsService = scope.ServiceProvider.GetRequiredService<IUserSettingsService>();
                return await userSettingsService.GetUserSettingsAsync(authState.User);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while retrieving user settings.", ex);
                throw;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Disposes of the resources used by the CustomAuthenticationStateProvider.
        /// </summary>
        /// <param name="disposing">True if disposing, false if finalizing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _subscription.Dispose();
                AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Validates the current authentication state.
        /// </summary>
        /// <param name="authenticationState">The current authentication state.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
        /// <returns>True if the authentication state is valid, false otherwise.</returns>
        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = authenticationState.User;
                if (user.Identity?.IsAuthenticated != true)
                {
                    return false;
                }

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }

                var cachedRolesJson = await _cache.GetStringAsync($"user_roles_{userId}", cancellationToken);
                if (!string.IsNullOrEmpty(cachedRolesJson))
                {
                    var cachedRoles = JsonSerializer.Deserialize<List<string>>(cachedRolesJson);
                    var identity = new ClaimsIdentity(user.Claims.ToList(), user.Identity.AuthenticationType);

                    foreach (var role in cachedRoles!)
                    {
                        if (!identity.HasClaim(ClaimTypes.Role, role))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    authenticationState = new AuthenticationState(new ClaimsPrincipal(identity));
                }

                await using var scope = _scopeFactory.CreateAsyncScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await ValidateSecurityStampAsync(userManager, authenticationState.User);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while validating authentication state.", ex);
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles changes to the authentication state.
        /// </summary>
        /// <param name="task">The task representing the new authentication state.</param>
        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            _authenticationStateTask = task;
        }

        /// <summary>
        /// Persists the current authentication state.
        /// </summary>
        private async Task OnPersistingAsync()
        {
            try
            {
                if (_authenticationStateTask is null)
                {
                    throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
                }

                var authenticationState = await _authenticationStateTask;
                var principal = authenticationState.User;

                if (principal.Identity?.IsAuthenticated == true)
                {
                    var userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
                    var name = principal.FindFirst(_options.ClaimsIdentity.UserNameClaimType)?.Value;
                    var email = principal.FindFirst(_options.ClaimsIdentity.EmailClaimType)?.Value;
                    var userRoles = principal.FindAll(_options.ClaimsIdentity.RoleClaimType);

                    if (userRoles != null)
                    {
                        var roles = userRoles.Select(r => r.Value).ToList();
                        _state.PersistAsJson(nameof(ApplicationUser), new ApplicationUser
                        {
                            UserId = userId,
                            UserName = name,
                            Email = email,
                            Roles = roles
                        });

                        await _cache.SetStringAsync($"user_roles_{userId}",
                            JsonSerializer.Serialize(roles),
                            new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while persisting authentication state.", ex);
            }
        }

        /// <summary>
        /// Validates the security stamp for the given user.
        /// </summary>
        /// <param name="userManager">The UserManager to use for validation.</param>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>True if the security stamp is valid, false otherwise.</returns>
        private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            try
            {
                var user = await userManager.GetUserAsync(principal);
                if (user is null)
                {
                    return false;
                }
                else if (!userManager.SupportsUserSecurityStamp)
                {
                    return true;
                }
                else
                {
                    var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
                    var userStamp = await userManager.GetSecurityStampAsync(user);
                    return principalStamp == userStamp;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while validating security stamp.", ex);
                return false;
            }
        }

        #endregion
    }
}