using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Domain.Entities.Authentication;

namespace Blazor.RCL.Infrastructure.Data
{
    /// <summary>
    /// Represents the database context for the application, extending IdentityDbContext.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #endregion

        #region DbSet Properties

        /// <summary>
        /// Gets or sets the UserSettings DbSet.
        /// </summary>
        public DbSet<UserSettings> UserSettings { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DistributedCache DbSet.
        /// </summary>
        public DbSet<DistributedCacheEntry> DistributedCache { get; set; } = null!;

        /// <summary>
        // Add this new DbSet for DataProtection
        /// <summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ToolsConfiguration DbSet.
        /// </summary>
        public DbSet<ToolsConfiguration> ToolsConfiguration { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ToolsConfiguration DbSet.
        /// </summary>
        public DbSet<LDAPServer> LDAPServers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for RemoteScripts.
        /// </summary>
        public DbSet<RemoteScript> RemoteScripts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for ServiceAccounts.
        /// </summary>
        public DbSet<ServiceAccount> ServiceAccounts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for ServiceAccounts.
        /// </summary>
        public DbSet<ServerHost> ServerHosts { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of AppDbContext.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <returns>A new instance of AppDbContext.</returns>
        public static AppDbContext Create(DbContextOptions<AppDbContext> options)
        {
            return new AppDbContext(options);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types exposed in DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add any additional model configurations here
            // For example:
            // modelBuilder.Entity<UserSettings>().HasIndex(u => u.UserId).IsUnique();
        }

        #endregion
    }
}