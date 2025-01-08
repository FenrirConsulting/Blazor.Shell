using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Web;

namespace Blazor.Shell.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication-related operations.
    /// </summary>
    [ApiController]
    [Route("/account")]
    public class AuthenticationController : Controller
    {
        #region Private Fields
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogHelper _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        #endregion

        #region Constructor
        public AuthenticationController(
            IAppConfiguration appConfiguration,
            ILogHelper logger,
            IHttpClientFactory httpClientFactory,
            IDistributedCache cache)
        {
            _appConfiguration = appConfiguration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient(_appConfiguration.CookieName);
            _cache = cache;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initiates the login process.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("cookielogin")]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            return await PerformAuthenticationRedirect("login", returnUrl);
        }

        /// <summary>
        /// Handles the callback after authentication.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("callback")]
        public IActionResult Callback(string returnUrl = "/")
        {
            returnUrl = _appConfiguration.ReturnURL;
            return Redirect(returnUrl);
        }

        /// <summary>
        /// Initiates the logout process.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            return await PerformAuthenticationRedirect("logout", returnUrl);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs the authentication redirect for login and logout.
        /// </summary>
        private async Task<IActionResult> PerformAuthenticationRedirect(string action, string returnUrl)
        {
            returnUrl = _appConfiguration.ReturnURL;
            var authenticationUrl = $"{_appConfiguration.AuthenticationAPIURL}{action}";
            var callbackUrl = $"{returnUrl}account/callback";

            var fullAuthenticationUrl = $"{authenticationUrl}?returnUrl={HttpUtility.UrlEncode(callbackUrl)}";

            try
            {
                return await Task.Run(() => Redirect(fullAuthenticationUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during {action}: ", ex);
                return StatusCode(500, $"An error occurred during the {action} process.");
            }
        }
        #endregion
    }
}