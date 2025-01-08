using System;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// A hosted service that initializes configuration data on application startup.
    /// </summary>
    public class ConfigurationInitializationService : IHostedService
    {
        #region Private Fields

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ConfigurationInitializationService class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for creating scopes and resolving dependencies.</param>
        /// <param name="logger">The logger for error and message logging.</param>
        public ConfigurationInitializationService(IServiceProvider serviceProvider, ILogHelper logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the hosted service and initializes configuration data.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the start operation.</param>
        /// <returns>A task that represents the asynchronous start operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var ldapServerList = _serviceProvider.GetRequiredService<LdapServerList>();
                var ldapRoleMappingConfig = _serviceProvider.GetRequiredService<LdapRoleMappingConfig>();
                var navLinksInfoList = _serviceProvider.GetRequiredService<NavLinksInfoList>();
                var remoteScriptList = _serviceProvider.GetRequiredService<RemoteScriptList>();
                var serviceAccountList = _serviceProvider.GetRequiredService<ServiceAccountList>();
                var serverHostList = _serviceProvider.GetRequiredService<ServerHostList>();

                await Task.WhenAll(
                    ldapServerList.ReloadServersAsync(),
                    ldapRoleMappingConfig.ReloadConfigAsync(),
                    navLinksInfoList.ReloadLinksAsync(),
                    remoteScriptList.ReloadScriptsAsync(),
                    serviceAccountList.ReloadAccountsAsync(),
                    serverHostList.ReloadHostsAsync()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred during configuration initialization.", ex);
                throw;
            }
        }

        /// <summary>
        /// Stops the hosted service.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the stop operation.</param>
        /// <returns>A task that represents the asynchronous stop operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        #endregion
    }
}