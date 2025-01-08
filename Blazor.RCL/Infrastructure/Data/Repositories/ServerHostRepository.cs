using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for server host configurations.
    /// </summary>
    public class ServerHostRepository : IServerHostRepository
    {
        #region Private Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ServerHostRepository class.
        /// </summary>
        public ServerHostRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<IEnumerable<ServerHost>> GetAllAsync()
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Set<ServerHost>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading server hosts", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ServerHost?> GetByIdAsync(int id)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Set<ServerHost>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting server host with ID {id}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task AddAsync(ServerHost serverHost)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                await context.Set<ServerHost>().AddAsync(serverHost);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding server host", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(ServerHost serverHost)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                context.Set<ServerHost>().Update(serverHost);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating server host with ID {serverHost.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(int id)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var serverHost = await context.Set<ServerHost>().FindAsync(id);
                if (serverHost != null)
                {
                    context.Set<ServerHost>().Remove(serverHost);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing server host with ID {id}", ex);
                throw;
            }
        }

        #endregion
    }
}
