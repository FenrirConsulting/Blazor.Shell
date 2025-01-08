using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for handling real-time updates to user claims via SignalR.
    /// </summary>
    public class ClaimsUpdateService : IClaimsUpdateService
    {
        #region Fields

        private readonly HubConnection _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ClaimsUpdateService class.
        /// </summary>
        /// <param name="httpContextAccessor">Accessor for the current HttpContext.</param>
        /// <param name="appConfiguration">Application configuration.</param>
        /// <param name="logger">Logger for error logging.</param>
        public ClaimsUpdateService(
            IHttpContextAccessor httpContextAccessor,
            IAppConfiguration appConfiguration,
            ILogHelper logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _connection = new HubConnectionBuilder()
                .WithUrl($"{_appConfiguration.AuthenticationAPIURL}claimsHub")
                .Build();

            _connection.On<string>("ReceiveClaimsUpdate", async (claimsJson) =>
            {
                await HandleClaimsUpdate(claimsJson);
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the SignalR connection.
        /// </summary>
        public async Task StartAsync()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to start SignalR connection", ex);
                throw;
            }
        }

        /// <summary>
        /// Stops the SignalR connection.
        /// </summary>
        public async Task StopAsync()
        {
            try
            {
                await _connection.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to stop SignalR connection", ex);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the claims update received from the SignalR hub.
        /// </summary>
        /// <param name="claimsJson">JSON string containing the updated claims.</param>
        private async Task HandleClaimsUpdate(string claimsJson)
        {
            try
            {
                var claims = JsonSerializer.Deserialize<List<Claim>>(claimsJson);
                if (claims == null)
                {
                    _logger.LogError("Failed to deserialize claims JSON", new InvalidOperationException("Deserialized claims is null"));
                    return;
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(_appConfiguration.SessionTimeout)
                };

                await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update claims", ex);
            }
        }

        #endregion
    }
}