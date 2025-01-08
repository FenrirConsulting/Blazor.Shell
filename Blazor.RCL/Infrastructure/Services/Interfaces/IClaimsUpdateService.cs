namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles real-time updates to user claims.
    /// </summary>
    public interface IClaimsUpdateService
    {
        /// <summary>
        /// Starts the claims update service.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the claims update service.
        /// </summary>
        Task StopAsync();
    }
}