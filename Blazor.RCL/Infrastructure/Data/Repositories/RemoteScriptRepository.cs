using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for RemoteScript operations using Entity Framework Core.
    /// </summary>
    public class RemoteScriptRepository : IRemoteScriptRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the RemoteScriptRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public RemoteScriptRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteScript>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RemoteScripts.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all RemoteScripts", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<RemoteScript?> GetByIdAsync(long id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RemoteScripts.FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting RemoteScript with ID: {id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AddAsync(RemoteScript script, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.RemoteScripts.AddAsync(script, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding RemoteScript", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(RemoteScript script, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.RemoteScripts.Update(script);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating RemoteScript with ID: {script.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(long id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var script = await context.RemoteScripts.FindAsync(new object[] { id }, token);
                if (script != null)
                {
                    context.RemoteScripts.Remove(script);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing RemoteScript with ID: {id}", ex);
                throw;
            }
        }
    }
}