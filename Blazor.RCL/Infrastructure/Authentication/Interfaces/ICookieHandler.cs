using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.Authentication.Interfaces
{
    /// <summary>
    /// Defines the contract for handling cookie-based authentication in outgoing HTTP requests.
    /// </summary>
    public interface ICookieHandler
    {
        /// <summary>
        /// Adds the authentication cookie to the outgoing HTTP request.
        /// </summary>
        /// <param name="request">The outgoing HTTP request to modify.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddCookieToRequestAsync(HttpRequestMessage request);
    }
}