namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Provides methods to check and set initialization status.
    /// </summary>
    public interface IInitializationService
    {
        /// <summary>
        /// Checks if the service has been initialized.
        /// </summary>
        /// <returns>True if initialized, otherwise false.</returns>
        bool IsInitialized();

        /// <summary>
        /// Sets the service as initialized.
        /// </summary>
        void SetInitialized();
    }
}