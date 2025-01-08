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
    /// Repository implementation for ServiceAccount operations using Entity Framework Core.
    /// </summary>
    public class ServiceAccountRepository : IServiceAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the ServiceAccountRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public ServiceAccountRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ServiceAccount>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ServiceAccounts.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all ServiceAccounts", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<ServiceAccount?> GetByIdAsync(long id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ServiceAccounts.FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ServiceAccount with ID: {id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AddAsync(ServiceAccount account, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.ServiceAccounts.AddAsync(account, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding ServiceAccount", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(ServiceAccount account, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.ServiceAccounts.Update(account);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating ServiceAccount with ID: {account.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(long id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var account = await context.ServiceAccounts.FindAsync(new object[] { id }, token);
                if (account != null)
                {
                    context.ServiceAccounts.Remove(account);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing ServiceAccount with ID: {id}", ex);
                throw;
            }
        }
    }
}