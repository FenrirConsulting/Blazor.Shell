using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for managing tool configurations.
    /// </summary>
    public interface IToolsConfigurationRepository
    {
        /// <summary>
        /// Retrieves a specific tool configuration asynchronously.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="setting">The setting name.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The tool configuration, or null if not found.</returns>
        Task<ToolsConfiguration?> GetByApplicationAndSettingAsync(string application, string setting, CancellationToken token = default);

        /// <summary>
        /// Retrieves all configurations for a specific application asynchronously.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>An enumerable of tool configurations.</returns>
        Task<IEnumerable<ToolsConfiguration>> GetAllByApplicationAsync(string application, CancellationToken token = default);

        /// <summary>
        /// Retrieves the value of a specific setting for an application asynchronously.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="setting">The setting name.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The setting value, or null if not found.</returns>
        Task<string?> GetValueAsync(string application, string setting, CancellationToken token = default);

        /// <summary>
        /// Sets a tool configuration asynchronously.
        /// </summary>
        /// <param name="toolsConfiguration">The tool configuration to set.</param>
        Task SetAsync(ToolsConfiguration toolsConfiguration);

        /// <summary>
        /// Removes a tool configuration asynchronously.
        /// </summary>
        /// <param name="config">The tool configuration to remove.</param>
        Task RemoveAsync(ToolsConfiguration config);
    }
}