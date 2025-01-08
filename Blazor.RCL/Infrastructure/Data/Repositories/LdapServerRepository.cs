using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for LDAPServer operations using Entity Framework Core.
    /// </summary>
    public class LDAPServerRepository : ILDAPServerRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the LDAPServerRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public LDAPServerRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LDAPServer>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.LDAPServers.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all LDAP servers", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<LDAPServer?> GetByIdAsync(long id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.LDAPServers.FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting LDAP server with ID: {id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AddAsync(LDAPServer server, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.LDAPServers.AddAsync(server, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding LDAP server", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(LDAPServer server, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.LDAPServers.Update(server);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating LDAP server with ID: {server.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(long id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var server = await context.LDAPServers.FindAsync(new object[] { id }, token);
                if (server != null)
                {
                    context.LDAPServers.Remove(server);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing LDAP server with ID: {id}", ex);
                throw;
            }
        }
    }
}