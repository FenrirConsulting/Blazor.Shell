using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages the list of Server Hosts.
    /// </summary>
    public class ServerHostList
    {
        #region Private Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogHelper _logger;
        private List<ServerHost> _hosts;
        private readonly ReaderWriterLockSlim _lock = new();
        private bool _isInitialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ServerHostList class.
        /// </summary>
        public ServerHostList(IServiceProvider serviceProvider, ILogHelper logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hosts = new List<ServerHost>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Server Hosts.
        /// </summary>
        public IReadOnlyList<ServerHost> Hosts
        {
            get
            {
                EnsureInitialized();
                _lock.EnterReadLock();
                try
                {
                    return _hosts.AsReadOnly();
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
        /// Reloads the Server Host list.
        /// </summary>
        public async Task ReloadHostsAsync()
        {
            try
            {
                List<ServerHost> newHosts;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IServerHostRepository>();
                    var hosts = await repository.GetAllAsync();
                    newHosts = new List<ServerHost>(hosts);
                }

                _lock.EnterWriteLock();
                try
                {
                    _hosts = newHosts;
                    _isInitialized = true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while reloading Server Host list.", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets server hosts filtered by environment.
        /// </summary>
        public List<ServerHost> GetServersByEnvironment(string environment)
        {
            EnsureInitialized();
            _lock.EnterReadLock();
            try
            {
                return _hosts
                    .Where(s => s.Environment.Equals(environment, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets server hosts filtered by role.
        /// </summary>
        public List<ServerHost> GetServersByRole(string role)
        {
            EnsureInitialized();
            _lock.EnterReadLock();
            try
            {
                return _hosts
                    .Where(s => s.Role.Equals(role, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets a list of unique environments.
        /// </summary>
        public List<string> GetEnvironments()
        {
            EnsureInitialized();
            _lock.EnterReadLock();
            try
            {
                return _hosts
                    .Select(s => s.Environment)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets a list of unique roles.
        /// </summary>
        public List<string> GetRoles()
        {
            EnsureInitialized();
            _lock.EnterReadLock();
            try
            {
                return _hosts
                    .Select(s => s.Role)
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        #region Private Methods

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
                            var repository = scope.ServiceProvider.GetRequiredService<IServerHostRepository>();
                            _hosts = repository.GetAllAsync().GetAwaiter().GetResult().ToList();
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

        #endregion
    }
}
