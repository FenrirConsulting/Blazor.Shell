using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for managing RemoteScript configurations.
    /// </summary>
    public interface IRemoteScriptRepository
    {
        /// <summary>
        /// Retrieves all RemoteScripts asynchronously.
        /// </summary>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>An enumerable of RemoteScript configurations.</returns>
        Task<IEnumerable<RemoteScript>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Retrieves a specific RemoteScript by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the RemoteScript.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The RemoteScript configuration, or null if not found.</returns>
        Task<RemoteScript?> GetByIdAsync(long id, CancellationToken token = default);

        /// <summary>
        /// Adds a new RemoteScript configuration asynchronously.
        /// </summary>
        /// <param name="script">The RemoteScript configuration to add.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task AddAsync(RemoteScript script, CancellationToken token = default);

        /// <summary>
        /// Updates an existing RemoteScript configuration asynchronously.
        /// </summary>
        /// <param name="script">The RemoteScript configuration to update.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task UpdateAsync(RemoteScript script, CancellationToken token = default);

        /// <summary>
        /// Removes a RemoteScript configuration asynchronously.
        /// </summary>
        /// <param name="id">The ID of the RemoteScript configuration to remove.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task RemoveAsync(long id, CancellationToken token = default);
    }
}