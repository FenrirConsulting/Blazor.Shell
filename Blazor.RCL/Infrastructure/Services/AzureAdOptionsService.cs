using Blazor.RCL.Domain.Entities.AzureAD;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.Configuration;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for retrieving and managing Azure AD configuration options.
    /// </summary>
    public class AzureAdOptionsService : IAzureAdOptions
    {
        #region Fields
        private readonly AzureAdOptions _options;
        private readonly AKeyLessOptions _aKeyLessOptions;
        private readonly ILogHelper _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AzureAdOptionsService class.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="registryHelper">Registry helper service.</param>
        /// <param name="logger">Logger instance.</param>
        public AzureAdOptionsService(IConfiguration configuration, IRegistryHelperService registryHelper, ILogHelper logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                _options = new AzureAdOptions();
                _aKeyLessOptions = new AKeyLessOptions();
                configuration.GetSection("AzureAd").Bind(_options);
                configuration.GetSection("AKeyLess").Bind(_aKeyLessOptions);

                // Get the ServicePrincipalRegistryKey from the registry
                _options.ServicePrincipalRegistryKey = registryHelper.GetRegistryValue(_options.ServicePrincipalRegistryKey!);
                _aKeyLessOptions.ACM1 = registryHelper.GetRegistryValue(_aKeyLessOptions.ACM1Path!);
                _aKeyLessOptions.ACM2 = registryHelper.GetRegistryValue(_aKeyLessOptions.ACM2Path!);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing AzureAdOptionsService", ex);
                throw;
            }
        }
        #endregion

        #region Public Properties
        /// <inheritdoc/>
        public string ClientId => _options.ClientId!;

        /// <inheritdoc/>
        public string TenantId => _options.TenantId!;

        /// <inheritdoc/>
        public string ApplicationRegistrationObjectId => _options.ApplicationRegistrationObjectId!;

        /// <inheritdoc/>
        public string ServicePrincipalObjectId => _options.ServicePrincipalObjectId!;

        /// <inheritdoc/>
        public string ServicePrincipalRegistryKey => _options.ServicePrincipalRegistryKey!;

        /// <inheritdoc/>
        public string ServicePrincipalAKeylessKey => _options.ServicePrincipalAKeylessKey!;

        /// <inheritdoc/>
        public IReadOnlyList<ApiPermission> ApiPermissions => _options.ApiPermissions!.AsReadOnly();

        /// <inheritdoc/>
        public string SharePointSiteId => _options.SharePointSiteId!;

        /// <inheritdoc/>
        public AKeyLessOptions AKeyLess => _aKeyLessOptions!;
        #endregion
    }
}
