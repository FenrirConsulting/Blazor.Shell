using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for managing LDAP server configurations.
    /// </summary>
    public interface ILDAPServerRepository
    {
        /// <summary>
        /// Retrieves all LDAP servers asynchronously.
        /// </summary>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>An enumerable of LDAP server configurations.</returns>
        Task<IEnumerable<LDAPServer>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Retrieves a specific LDAP server by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the LDAP server.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The LDAP server configuration, or null if not found.</returns>
        Task<LDAPServer?> GetByIdAsync(long id, CancellationToken token = default);

        /// <summary>
        /// Adds a new LDAP server configuration asynchronously.
        /// </summary>
        /// <param name="server">The LDAP server configuration to add.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task AddAsync(LDAPServer server, CancellationToken token = default);

        /// <summary>
        /// Updates an existing LDAP server configuration asynchronously.
        /// </summary>
        /// <param name="server">The LDAP server configuration to update.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task UpdateAsync(LDAPServer server, CancellationToken token = default);

        /// <summary>
        /// Removes an LDAP server configuration asynchronously.
        /// </summary>
        /// <param name="id">The ID of the LDAP server configuration to remove.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        Task RemoveAsync(long id, CancellationToken token = default);
    }
}