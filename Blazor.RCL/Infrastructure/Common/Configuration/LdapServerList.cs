using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Application.Models;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages the list of LDAP servers.
    /// </summary>
    public class LdapServerList
    {
        #region Private Fields
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogHelper _logger;
        private List<LDAPServer> _servers;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _isInitialized = false;

        public event EventHandler? ServersUpdated;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the LdapServerList class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scopes and resolving dependencies.</param>
        /// <param name="logger">The logger for error and message logging.</param>
        public LdapServerList(IServiceProvider serviceProvider, ILogHelper logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _servers = new List<LDAPServer>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of LDAP servers.
        /// </summary>
        public IReadOnlyList<LDAPServer> Servers
        {
            get
            {
                EnsureInitialized();
                _lock.EnterReadLock();
                try
                {
                    return _servers.AsReadOnly();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reloads the LDAP server list.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReloadServersAsync()
        {
            List<LDAPServer> newServers;
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<ILDAPServerRepository>();
                    newServers = await LoadServersAsync(repository);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while reloading LDAP server list.", ex);
                throw;
            }

            _lock.EnterWriteLock();
            try
            {
                _servers = newServers;
                _isInitialized = true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures that the server list is initialized.
        /// </summary>
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
                            var repository = scope.ServiceProvider.GetRequiredService<ILDAPServerRepository>();
                            _servers = LoadServersAsync(repository).GetAwaiter().GetResult();
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

        /// <summary>
        /// Loads the LDAP server list from the repository.
        /// </summary>
        /// <param name="repository">The LDAP server repository.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task<List<LDAPServer>> LoadServersAsync(ILDAPServerRepository repository)
        {
            try
            {
                var ldapServers = await repository.GetAllAsync();
                return ldapServers?.ToList() ?? new List<LDAPServer>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading LDAP servers", ex);
                throw;
            }
        }

        protected virtual void OnServersUpdated()
        {
            ServersUpdated?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}