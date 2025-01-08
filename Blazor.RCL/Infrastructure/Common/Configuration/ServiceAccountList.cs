using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages the list of Service Accounts.
    /// </summary>
    public class ServiceAccountList
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogHelper _logger;
        private List<ServiceAccount> _accounts;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes a new instance of the ServiceAccountList class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scopes and resolving dependencies.</param>
        /// <param name="logger">The logger for error and message logging.</param>
        public ServiceAccountList(IServiceProvider serviceProvider, ILogHelper logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accounts = new List<ServiceAccount>();
        }

        /// <summary>
        /// Gets the list of Service Accounts.
        /// </summary>
        public IReadOnlyList<ServiceAccount> Accounts
        {
            get
            {
                EnsureInitialized();
                _lock.EnterReadLock();
                try
                {
                    return _accounts.AsReadOnly();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Reloads the Service Account list.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReloadAccountsAsync()
        {
            try
            {
                List<ServiceAccount> newAccounts;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IServiceAccountRepository>();
                    var registryHelperService = scope.ServiceProvider.GetRequiredService<IRegistryHelperService>();
                    newAccounts = await LoadAccountsAsync(repository, registryHelperService);
                }

                _lock.EnterWriteLock();
                try
                {
                    _accounts = newAccounts;
                    _isInitialized = true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while reloading Service Account list.", ex);
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
                            var repository = scope.ServiceProvider.GetRequiredService<IServiceAccountRepository>();
                            var registryHelperService = scope.ServiceProvider.GetRequiredService<IRegistryHelperService>();
                            _accounts = LoadAccountsAsync(repository, registryHelperService).GetAwaiter().GetResult();
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

        private async Task<List<ServiceAccount>> LoadAccountsAsync(IServiceAccountRepository repository, IRegistryHelperService registryHelperService)
        {
            try
            {
                var serviceAccounts = await repository.GetAllAsync();
                if (serviceAccounts == null)
                {
                    _logger.LogMessage("Retrieved Service Accounts list is null.");
                    return new List<ServiceAccount>();
                }

                var accounts = new List<ServiceAccount>();
                foreach (var account in serviceAccounts)
                {
                    if (account.KeyType == "Registry")
                    {
                        try
                        {
                            string registryValue = registryHelperService.GetRegistryValue(account.KeyPath!);
                            account.KeyPath = registryValue;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to retrieve registry value for account {account.SAM}", ex);
                            // Optionally, you might want to skip this account or handle the error differently
                        }
                    }
                    accounts.Add(account);
                }
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading Service Accounts", ex);
                throw;
            }
        }
    }
}