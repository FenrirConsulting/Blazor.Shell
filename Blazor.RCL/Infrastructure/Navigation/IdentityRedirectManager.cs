using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Blazor.RCL.Infrastructure.Navigation
{
    /// <summary>
    /// Manages redirects for identity-related operations in the application.
    /// </summary>
    public sealed class IdentityRedirectManager
    {
        #region Constants

        /// <summary>
        /// The name of the cookie used to store identity status messages.
        /// </summary>
        public const string StatusCookieName = "Identity.StatusMessage";

        #endregion

        #region Fields

        private readonly NavigationManager _navigationManager;

        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            MaxAge = TimeSpan.FromSeconds(5),
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the IdentityRedirectManager class.
        /// </summary>
        /// <param name="navigationManager">The navigation manager used for redirects.</param>
        public IdentityRedirectManager(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Redirects to the specified URI.
        /// </summary>
        /// <param name="uri">The URI to redirect to.</param>
        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= "";
            // Prevent open redirects.
            if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = _navigationManager.ToBaseRelativePath(uri);
            }
            // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
            // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
            _navigationManager.NavigateTo(uri);
            throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
        }

        /// <summary>
        /// Redirects to the specified URI with query parameters.
        /// </summary>
        /// <param name="uri">The URI to redirect to.</param>
        /// <param name="queryParameters">The query parameters to include in the redirect.</param>
        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
            var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        /// <summary>
        /// Redirects to the specified URI with a status message.
        /// </summary>
        /// <param name="uri">The URI to redirect to.</param>
        /// <param name="message">The status message to include in the redirect.</param>
        /// <param name="context">The HTTP context.</param>
        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
            RedirectTo(uri);
        }

        /// <summary>
        /// Redirects to the current page.
        /// </summary>
        [DoesNotReturn]
        public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

        /// <summary>
        /// Redirects to the current page with a status message.
        /// </summary>
        /// <param name="message">The status message to include in the redirect.</param>
        /// <param name="context">The HTTP context.</param>
        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
            => RedirectToWithStatus(CurrentPath, message, context);

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the current path of the application.
        /// </summary>
        private string CurrentPath => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);

        #endregion
    }
}