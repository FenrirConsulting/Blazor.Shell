using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages the mapping of LDAP roles to application roles.
    /// </summary>
    public class LdapRoleMappingConfig
    {
        #region Private Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogHelper _logger;
        private Dictionary<string, List<string>> _ldapRoleMappings;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _isInitialized = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the LdapRoleMappingConfig class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scopes and resolving dependencies.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="logger">The logger for error and message logging.</param>
        public LdapRoleMappingConfig(
        IServiceProvider serviceProvider,
        IAppConfiguration appConfiguration,
        ILogHelper logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ldapRoleMappings = new Dictionary<string, List<string>>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the LDAP role mappings.
        /// </summary>
        public IReadOnlyDictionary<string, List<string>> LdapRoleMappings
        {
            get
            {
                EnsureInitialized();
                _lock.EnterReadLock();
                try
                {
                    return new Dictionary<string, List<string>>(_ldapRoleMappings);
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
        /// Reloads the LDAP role mapping configuration.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReloadConfigAsync()
        {
            try
            {
                Dictionary<string, List<string>> newMappings;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IToolsConfigurationRepository>();
                    newMappings = await LoadConfigAsync(repository);
                }

                _lock.EnterWriteLock();
                try
                {
                    _ldapRoleMappings = newMappings;
                    _isInitialized = true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while reloading LDAP role mapping configuration.", ex);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures that the configuration is initialized.
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
                            var repository = scope.ServiceProvider.GetRequiredService<IToolsConfigurationRepository>();
                            _ldapRoleMappings = LoadConfigAsync(repository).GetAwaiter().GetResult();
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
        /// Loads the LDAP role mapping configuration from the repository.
        /// </summary>
        /// <param name="repository">The tools configuration repository.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task<Dictionary<string, List<string>>> LoadConfigAsync(IToolsConfigurationRepository repository)
        {
            try
            {
                var jsonValue = await repository.GetValueAsync(_appConfiguration.AppName, "LdapRoleMappings");
                if (string.IsNullOrEmpty(jsonValue))
                {
                    _logger.LogMessage("LdapRoleMappingConfig JSON value is null or empty.");
                    return new Dictionary<string, List<string>>();
                }

                var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonValue);
                return deserializedDictionary ?? new Dictionary<string, List<string>>();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error deserializing LdapRoleMappingConfig", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading LdapRoleMappingConfig", ex);
                throw;
            }
        }

        #endregion
    }
}