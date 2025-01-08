using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages the list of Remote Scripts.
    /// </summary>
    public class RemoteScriptList
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogHelper _logger;
        private List<RemoteScript> _scripts;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the RemoteScriptList class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scopes and resolving dependencies.</param>
        /// <param name="logger">The logger for error and message logging.</param>
        public RemoteScriptList(IServiceProvider serviceProvider, ILogHelper logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scripts = new List<RemoteScript>();
        }

        /// <summary>
        /// Gets the list of Remote Scripts.
        /// </summary>
        public IReadOnlyList<RemoteScript> Scripts
        {
            get
            {
                EnsureInitialized();
                _lock.EnterReadLock();
                try
                {
                    return _scripts.AsReadOnly();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Reloads the Remote Script list.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReloadScriptsAsync()
        {
            try
            {
                List<RemoteScript> newScripts;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IRemoteScriptRepository>();
                    newScripts = await LoadScriptsAsync(repository);
                }

                _lock.EnterWriteLock();
                try
                {
                    _scripts = newScripts;
                    _isInitialized = true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while reloading Remote Script list.", ex);
                throw;
            }
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                _lock.EnterUpgradeableReadLock();
                try
                {
                    if (!_isInitialized)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repository = scope.ServiceProvider.GetRequiredService<IRemoteScriptRepository>();
                            _scripts = LoadScriptsAsync(repository).GetAwaiter().GetResult();
                        }
                        _isInitialized = true;
                    }
                }
                finally
                {
                    _lock.ExitUpgradeableReadLock();
                }
            }
        }

        private async Task<List<RemoteScript>> LoadScriptsAsync(IRemoteScriptRepository repository)
        {
            try
            {
                var remoteScripts = await repository.GetAllAsync();
                return remoteScripts?.ToList() ?? new List<RemoteScript>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading Remote Scripts", ex);
                throw;
            }
        }
    }
}