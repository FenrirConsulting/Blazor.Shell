using System.Collections.Generic;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Provides methods to retrieve values from the Windows Registry.
    /// </summary>
    public interface IRegistryHelperService
    {
        /// <summary>
        /// Retrieves the JWT secret key from the registry.
        /// </summary>
        /// <returns>The JWT secret key.</returns>
        string GetJwtSecretKey();

        /// <summary>
        /// Retrieves multiple registry values based on provided property names.
        /// </summary>
        /// <param name="propertyNames">Collection of property names to retrieve.</param>
        /// <returns>Dictionary of property names and their corresponding registry values.</returns>
        Dictionary<string, string> GetRegistryValues(IEnumerable<string> propertyNames);

        string GetRegistryValue(string keyPath);
    }
}