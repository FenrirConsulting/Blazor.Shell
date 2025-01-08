using Blazor.RCL.Infrastructure.Services.Interfaces;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Provides thread-safe initialization services.
    /// </summary>
    public class InitializationService : IInitializationService
    {
        #region Fields

        private bool _initialized;
        private readonly object _lock = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the service has been initialized.
        /// </summary>
        /// <returns>True if initialized, otherwise false.</returns>
        public bool IsInitialized()
        {
            lock (_lock)
            {
                return _initialized;
            }
        }

        /// <summary>
        /// Sets the service as initialized.
        /// </summary>
        public void SetInitialized()
        {
            lock (_lock)
            {
                _initialized = true;
            }
        }

        #endregion
    }
}