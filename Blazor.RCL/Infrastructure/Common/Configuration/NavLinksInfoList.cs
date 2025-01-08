using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.RCL.Application.Models;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages the list of navigation menu links.
    /// </summary>
    public class NavLinksInfoList
    {
        #region Private Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly IAppConfiguration _appConfiguration;
        private readonly ILogHelper _logger;
        private List<NavLinkInfo> _links;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _isInitialized = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the NavLinksInfoList class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scopes and resolving dependencies.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="logger">The logger for error and message logging.</param>
        public NavLinksInfoList(
        IServiceProvider serviceProvider,
        IAppConfiguration appConfiguration,
        ILogHelper logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _links = new List<NavLinkInfo>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of navigation links.
        /// </summary>
        public IReadOnlyList<NavLinkInfo> Links
        {
            get
            {
                EnsureInitialized();
                _lock.EnterReadLock();
                try
                {
                    return _links.AsReadOnly();
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
        /// Reloads the navigation links list.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ReloadLinksAsync()
        {
            try
            {
                List<NavLinkInfo> newLinks;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IToolsConfigurationRepository>();
                    newLinks = await LoadLinksAsync(repository);
                }

                _lock.EnterWriteLock();
                try
                {
                    _links = newLinks;
                    _isInitialized = true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while reloading navigation links.", ex);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures that the links list is initialized.
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
                            _links = LoadLinksAsync(repository).GetAwaiter().GetResult();
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
        /// Loads the navigation links from the repository.
        /// </summary>
        /// <param name="repository">The tools configuration repository.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task<List<NavLinkInfo>> LoadLinksAsync(IToolsConfigurationRepository repository)
        {
            try
            {
                var jsonValue = await repository.GetValueAsync(_appConfiguration.AppName, "NavLinks");
                if (string.IsNullOrEmpty(jsonValue))
                {
                    _logger.LogMessage("NavLinks JSON value is null or empty.");
                    return new List<NavLinkInfo>();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var deserializedList = JsonSerializer.Deserialize<List<NavLinkInfo>>(jsonValue, options);
                return deserializedList ?? new List<NavLinkInfo>();
            }
            catch (JsonException ex)
            {
                _logger.LogError("Error deserializing NavLinks", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading NavLinks", ex);
                throw;
            }
        }

        #endregion
    }
}