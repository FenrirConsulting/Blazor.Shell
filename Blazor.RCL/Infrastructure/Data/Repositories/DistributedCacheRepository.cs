using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Authentication;
using Microsoft.EntityFrameworkCore;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for distributed cache operations using Entity Framework Core.
    /// </summary>
    public class DistributedCacheRepository : IDistributedCacheRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DistributedCacheRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public DistributedCacheRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a cached item by its key.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The cached item's value, or null if not found or expired.</returns>
        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var entry = await context.DistributedCache.FindAsync(new object[] { key }, token);
                if (entry != null && entry.ExpiresAtTime > DateTimeOffset.UtcNow)
                {
                    return entry.Value;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting cache item with key: {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets a cached item with the specified key and value.
        /// </summary>
        /// <param name="key">The key of the item to set.</param>
        /// <param name="value">The value of the item to set.</param>
        /// <param name="absoluteExpireTime">The absolute expiration time.</param>
        /// <param name="unusedExpireTime">The sliding expiration time.</param>
        /// <param name="token">Cancellation token.</param>
        public async Task SetAsync(string key, byte[] value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var entry = await context.DistributedCache.FindAsync(new object[] { key }, token);
                if (entry == null)
                {
                    entry = new DistributedCacheEntry { Id = key };
                    context.DistributedCache.Add(entry);
                }
                entry.Value = value;
                entry.ExpiresAtTime = DateTimeOffset.UtcNow.Add(absoluteExpireTime ?? TimeSpan.FromMinutes(30));
                entry.SlidingExpirationInSeconds = (long?)unusedExpireTime?.TotalSeconds;
                entry.AbsoluteExpiration = absoluteExpireTime.HasValue ? DateTimeOffset.UtcNow.Add(absoluteExpireTime.Value) : null;
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while setting cache item with key: {key}", ex);
                throw;
            }
        }

        /// <summary>
        /// Removes a cached item by its key.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <param name="token">Cancellation token.</param>
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var entry = await context.DistributedCache.FindAsync(new object[] { key }, token);
                if (entry != null)
                {
                    context.DistributedCache.Remove(entry);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing cache item with key: {key}", ex);
                throw;
            }
        }

        #endregion
    }
}