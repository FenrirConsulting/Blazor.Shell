using System;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Common
{
    /// <summary>
    /// Custom implementation of IDistributedCache that uses IDistributedCacheRepository.
    /// </summary>
    public class CustomDistributedCache : IDistributedCache
    {
        #region Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<ILogHelper> _lazyLogger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomDistributedCache class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        public CustomDistributedCache(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _lazyLogger = new Lazy<ILogHelper>(() => _serviceProvider.GetRequiredService<ILogHelper>());
        }

        #endregion

        #region Properties

        private ILogHelper Logger => _lazyLogger.Value;

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public byte[] Get(string key)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                return repository.GetAsync(key).GetAwaiter().GetResult()!;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while getting cache item with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                return await repository.GetAsync(key, token);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while getting cache item asynchronously with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                repository.SetAsync(key, value, options.AbsoluteExpirationRelativeToNow, options.SlidingExpiration).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while setting cache item with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                await repository.SetAsync(key, value, options.AbsoluteExpirationRelativeToNow, options.SlidingExpiration, token);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while setting cache item asynchronously with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public void Refresh(string key)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                var value = repository.GetAsync(key).GetAwaiter().GetResult();
                if (value != null)
                {
                    repository.SetAsync(key, value).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while refreshing cache item with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                var value = await repository.GetAsync(key, token);
                if (value != null)
                {
                    await repository.SetAsync(key, value, null, null, token);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while refreshing cache item asynchronously with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                repository.RemoveAsync(key).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while removing cache item with key: {key}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IDistributedCacheRepository>();
                await repository.RemoveAsync(key, token);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error occurred while removing cache item asynchronously with key: {key}", ex);
                throw;
            }
        }

        #endregion
    }
}