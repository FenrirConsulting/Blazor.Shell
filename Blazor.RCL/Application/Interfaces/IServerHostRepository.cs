using Blazor.RCL.Domain.Entities;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for managing server host configurations.
    /// </summary>
    public interface IServerHostRepository
    {
        /// <summary>
        /// Gets all server hosts.
        /// </summary>
        /// <returns>Collection of server hosts.</returns>
        Task<IEnumerable<ServerHost>> GetAllAsync();

        /// <summary>
        /// Gets a server host by its identifier.
        /// </summary>
        /// <param name="id">The server host identifier.</param>
        /// <returns>The server host if found; otherwise, null.</returns>
        Task<ServerHost?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new server host.
        /// </summary>
        /// <param name="serverHost">The server host to add.</param>
        Task AddAsync(ServerHost serverHost);

        /// <summary>
        /// Updates an existing server host.
        /// </summary>
        /// <param name="serverHost">The server host to update.</param>
        Task UpdateAsync(ServerHost serverHost);

        /// <summary>
        /// Removes a server host.
        /// </summary>
        /// <param name="id">The identifier of the server host to remove.</param>
        Task RemoveAsync(int id);
    }
}
