using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Newtonsoft.Json.Bson;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Retrieves and manages configuration values from appsettings.json and the registry.
    /// </summary>
    public class ServerAppConfiguration : IAppConfiguration
    {
        #region Private Fields

        private readonly IConfiguration _configuration;
        private readonly IRegistryHelperService _registryHelper;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ServerAppConfiguration class.
        /// </summary>
        /// <param name="configuration">The configuration interface.</param>
        /// <param name="registryHelper">The registry helper service.</param>
        /// <param name="logger">The logging helper.</param>
        public ServerAppConfiguration(IConfiguration configuration, IRegistryHelperService registryHelper, ILogHelper logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _registryHelper = registryHelper ?? throw new ArgumentNullException(nameof(registryHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                LoadDynamicProperties();
                ConnectionString = BuildConnectionString();
                BuildAdditionalConnectionStrings();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing ServerAppConfiguration", ex);
                throw;
            }
        }

        #endregion

        #region Public Properties

        public string BaseUrl => _configuration["BaseUrl"]!;
        public string AppName => _configuration["AppName"]!;
        public string AppLabel => _configuration["AppLabel"]!;
        public string EnvironmentLoaded => _configuration["EnvironmentLoaded"]!;
        public string DomainSite => _configuration["DomainSite"]!;
        public string AuthenticationAPIURL => _configuration["AuthenticationAPIURL"]!;
        public string ReturnURL => _configuration["ReturnURL"]!;
        public Dictionary<string, string> RegistryValues { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DynamicValues { get; private set; } = new Dictionary<string, string>();
        public string ConnectionString { get; private set; }
        public string CookieName => _configuration["CookieName"]!;
        public double SessionTimeout => double.Parse(_configuration["SessionTimeout"]!);
        public Dictionary<string, string> AdditionalConnectionStrings { get; private set; } = new Dictionary<string, string>();

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads dynamic properties from the registry based on configuration.
        /// </summary>
        private void LoadDynamicProperties()
        {
            try
            {
                // Load in Registry Configuration Propertyies
                var registryPropertyNames = _configuration.GetSection("RegistryConfiguration").GetChildren();
                var registryPropertyNamesList = new List<string>();
                foreach (var property in registryPropertyNames)
                {
                    registryPropertyNamesList.Add(property.Key);
                }
                RegistryValues = _registryHelper.GetRegistryValues(registryPropertyNamesList);

                // Load Remaining Dynamic Properties
                var dynamicPropertyNames = _configuration.GetSection("DynamicProperties").GetChildren();
                var dynamicPropertyNamesList = new List<string>();
                foreach (var property in dynamicPropertyNames)
                {
                    dynamicPropertyNamesList.Add(property.Key);
                }
                DynamicValues = _configuration.GetSection("DynamicProperties").Get<Dictionary<string, string>>()!;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while loading dynamic properties", ex);
                throw;
            }
        }

        /// <summary>
        /// Builds the connection string using configuration and registry values.
        /// </summary>
        /// <returns>The constructed connection string.</returns>
        private string BuildConnectionString(string databaseName = null!)
        {
            try
            {
                var server = _configuration["Database:Server"];
                var database = databaseName ?? _configuration["Database:Database"];
                var username = _configuration["Database:Username"];
                var password = RegistryValues.TryGetValue("SQLConnectionKey", out var pwd) ? pwd : string.Empty;

                var builder = new StringBuilder();
                builder.Append($"Server={server};");
                builder.Append($"initial catalog={database};");
                builder.Append("integrated security=False;");
                builder.Append($"User Id={username};");
                builder.Append($"password={password};");

                return builder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while building connection string", ex);
                throw;
            }
        }

        /// <summary>
        /// Builds the dynamic list of additional databases from configuration.
        /// </summary>
        /// <returns>The constructed connection string.</returns>
        private void BuildAdditionalConnectionStrings()
        {
            try
            {
                var additionalDatabases = _configuration.GetSection("AdditionalDatabases").Get<List<string>>();
                if (additionalDatabases != null)
                {
                    foreach (var database in additionalDatabases)
                    {
                        AdditionalConnectionStrings[database] = BuildConnectionString(database);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while building connection string", ex);
                throw;
            }
        }

        #endregion
    }
}