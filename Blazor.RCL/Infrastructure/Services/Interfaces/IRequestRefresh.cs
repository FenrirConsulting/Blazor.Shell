using System;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Provides methods to request and handle refresh events.
    /// </summary>
    public interface IRequestRefresh
    {
        /// <summary>
        /// Event triggered when a refresh is requested.
        /// </summary>
        event Action RefreshRequested;

        /// <summary>
        /// Invokes the RefreshRequested event to trigger a refresh.
        /// </summary>
        void CallRequestRefresh();
    }
}