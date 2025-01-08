using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for retrieving values from Windows Registry Configuration Paths.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class RegistryHelperService : IRegistryHelperService
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RegistryHelperService class.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="logger">Logger instance.</param>
        public RegistryHelperService(IConfiguration configuration, ILogHelper logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the JWT secret key from the registry.
        /// </summary>
        /// <returns>The JWT secret key.</returns>
        public string GetJwtSecretKey()
        {
            return GetRegistryValue(_configuration["Jwt:KeyPath"]!);
        }

        /// <summary>
        /// Retrieves multiple registry values based on provided property names.
        /// </summary>
        /// <param name="propertyNames">Collection of property names to retrieve.</param>
        /// <returns>Dictionary of property names and their corresponding registry values.</returns>
        public Dictionary<string, string> GetRegistryValues(IEnumerable<string> propertyNames)
        {
            var registryValues = new Dictionary<string, string>();

            foreach (var propertyName in propertyNames)
            {
                string keyPath = _configuration[$"RegistryConfiguration:{propertyName}"]!;
                if (!string.IsNullOrEmpty(keyPath))
                {
                    var value = GetRegistryValue(keyPath);
                    registryValues.Add(propertyName, value);
                }
                else
                {
                    _logger.LogMessage($"Registry path for {propertyName} is not configured. Skipping.");
                }
            }

            return registryValues;
        }

        /// <summary>
        /// Retrieves a single registry value based on the provided key path.
        /// </summary>
        /// <param name="keyPath">Registry key path.</param>
        /// <returns>The registry value as a string.</returns>
        /// <exception cref="Exception">Thrown when the registry value is not found or an error occurs.</exception>
        public string GetRegistryValue(string keyPath)
        {
            if (string.IsNullOrEmpty(keyPath))
            {
                _logger.LogMessage("Registry path or key name is not configured.");
                throw new ArgumentException("Registry path or key name is not configured.", nameof(keyPath));
            }

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        var value = key.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(value))
                        {
                            return value;
                        }
                    }
                }

                _logger.LogMessage($"Value not found in Registry: {keyPath}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetRegistryValue", ex);
                return string.Empty;
            }
        }

        #endregion
    }
}