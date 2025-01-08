using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using System.Security.Authentication;

namespace Blazor.RCL.Infrastructure.Authentication
{
    /// <summary>
    /// Handles the creation and addition of a cookie header for communication between Domain endpoints.
    /// Implements error handling for connection issues in load-balanced environments.
    /// </summary>
    public class CookieHandler : DelegatingHandler, ICookieHandler
    {
        #region Private Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogHelper _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the CookieHandler class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HttpContext.</param>
        /// <param name="appConfiguration">Provides application configuration settings.</param>
        /// <param name="logger">Provides logging functionality.</param>
        public CookieHandler(
            IHttpContextAccessor httpContextAccessor,
            IAppConfiguration appConfiguration,
            ILogHelper logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the authentication cookie to the outgoing HTTP request.
        /// </summary>
        /// <param name="request">The outgoing HTTP request to modify.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AddCookieToRequestAsync(HttpRequestMessage request)
        {
            try
            {
                var cookieName = _appConfiguration.CookieName;
                var cookie = _httpContextAccessor.HttpContext?.Request.Cookies[cookieName];

                if (!string.IsNullOrEmpty(cookie))
                {
                    request.Headers.Add("Cookie", $"{cookieName}={cookie}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding cookie to outgoing request.", ex);
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Intercepts the outgoing request to add the cookie header if available.
        /// Implements error handling for connection issues.
        /// </summary>
        /// <param name="request">The outgoing HTTP request.</param>
        /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
        /// <returns>The HTTP response message.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await AddCookieToRequestAsync(request);

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP request error occurred.", ex);
                throw; // Re-throw the exception after logging
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a clone of the original HttpRequestMessage.
        /// </summary>
        /// <param name="request">The original HTTP request to clone.</param>
        /// <returns>A new HttpRequestMessage with the same properties as the original.</returns>
        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            // Copy the request's content (if present)
            if (request.Content != null)
            {
                clone.Content = new StreamContent(await request.Content.ReadAsStreamAsync());
                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.Add(header.Key, header.Value);
                }
            }

            // Copy the request options
            foreach (var option in request.Options)
            {
                clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
        #endregion
    }
}