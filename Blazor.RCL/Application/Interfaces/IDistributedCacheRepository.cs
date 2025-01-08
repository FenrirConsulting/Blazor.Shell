using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for interacting with a distributed cache.
    /// </summary>
    public interface IDistributedCacheRepository
    {
        /// <summary>
        /// Retrieves a cached item asynchronously.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The cached item as a byte array, or null if not found.</returns>
        Task<byte[]?> GetAsync(string key, CancellationToken token = default);

        /// <summary>
        /// Sets an item in the cache asynchronously.
        /// </summary>
        /// <param name="key">The key of the item to set.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="absoluteExpireTime">Optional. The absolute expiration time for the cached item.</param>
        /// <param name="unusedExpireTime">Optional. The sliding expiration time for the cached item.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task SetAsync(string key, byte[] value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null, CancellationToken token = default);

        /// <summary>
        /// Removes an item from the cache asynchronously.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task RemoveAsync(string key, CancellationToken token = default);
    }
}