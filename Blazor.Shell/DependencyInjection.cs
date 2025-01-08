using MudBlazor.Extensions;
using MudBlazor.Services;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services;
using Blazor.RCL.Infrastructure.Authentication;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Navigation;
using Microsoft.AspNetCore.Authentication;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.NLog.LogService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Blazor.RCL.Infrastructure.Data;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Blazor.RCL.Infrastructure.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using Blazor.RCL.Application.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Blazor.Shell
{
    /// <summary>
    /// Provides dependency injection and configuration methods for the server application.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds server UI services to the service collection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="config">The configuration object.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddServerUI(this IServiceCollection services, IConfiguration config)
        {
            StandardServices(services, config);

            /// <summary>
            /// Add application specific services not included in standard service set from WebCoreUtility
            /// </summary>
            
            /// Static Cofiguration Services
            services.AddSingleton(new ViewModuleList(config));

            return services;
        }

        /// <summary>
        /// Configures the server application.
        /// </summary>
        /// <param name="app">The WebApplication to configure.</param>
        /// <param name="config">The configuration object.</param>
        /// <returns>The configured WebApplication.</returns>
        public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
        {
            StandardMiddleware(app, config);

            /// <summary>
            /// Add application specific configuration  not included in standard configuration set from WebCoreUtility
            /// </summary>

            return app;
        }

        private static void StandardServices(IServiceCollection services, IConfiguration config)
        {
            #region Application Services
            // Core application services
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddScoped<ErrorHandlerService>();

            services.AddRazorComponents()
                    .AddInteractiveServerComponents();

            services.AddServerSideBlazor(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
                options.MaxBufferedUnacknowledgedRenderBatches = 10;
                options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(60);
                options.DisconnectedCircuitMaxRetained = 100; // Limit the number of disconnected circuits
            })
            .AddHubOptions(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
            });

            // Additional Services
            services.AddHttpClient();
            services.AddScoped<IRequestRefresh, RequestRefresh>();
            services.AddScoped<IdentityRedirectManager>();

            // NLog Services for logging
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ILogHelper, LogHelper>();

            // MudBlazor UI Components Added
            services.AddMudServices();
            services.AddMudBlazorSnackbar();
            services.AddMudServicesWithExtensions();
            services.AddScoped<IThemeService, ThemeService>();
            #endregion

            #region Distributed Cache Services
            // Configure distributed cache for improved performance
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();
            services.AddSingleton<IDistributedCache>(sp => new CustomDistributedCache(sp));
            #endregion

            #region Static Configuration Services
            // Services for managing static configuration
            services.AddSingleton<LdapServerList>();
            services.AddSingleton<LdapRoleMappingConfig>();
            services.AddSingleton<NavLinksInfoList>();
            services.AddSingleton<RemoteScriptList>();
            services.AddSingleton<ServiceAccountList>();
            services.AddSingleton<ServerHostList>();
            services.AddSingleton<IRegistryHelperService, RegistryHelperService>();

            // Holds Contents of AppSettings.JSON in a Service Class
            services.AddSingleton<IAppConfiguration>(sp =>
            {
                var registryHelper = sp.GetRequiredService<IRegistryHelperService>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                return new ServerAppConfiguration(config, registryHelper, logHelper);
            });

            // Holds Azure AD Options in a Service Class
            services.AddSingleton<IAzureAdOptions>(sp =>
            {
                var registryHelper = sp.GetRequiredService<IRegistryHelperService>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                return new AzureAdOptionsService(config, registryHelper, logHelper);
            });

            services.AddScoped<IUserSettingsService, UserSettingsService>();
            #endregion

            #region Database Services
            // Add DbContext with connection string from appConfiguration
            var appConfiguration = services.BuildServiceProvider().GetRequiredService<IAppConfiguration>();
            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(appConfiguration.ConnectionString);
                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name
                    && level == LogLevel.Information)));
            });

            // Table Repository Services for data access
            services.AddScoped<ILDAPServerRepository, LDAPServerRepository>();
            services.AddScoped<IToolsConfigurationRepository, ToolsConfigurationRepository>();
            services.AddScoped<IRemoteScriptRepository, RemoteScriptRepository>();
            services.AddScoped<IServiceAccountRepository, ServiceAccountRepository>();
            services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
            services.AddScoped<IServerHostRepository, ServerHostRepository>();
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();

            // Preload Configuration into Static Classes before Authentication is Called
            services.AddHostedService<ConfigurationInitializationService>();
            #endregion

            #region Authentication Services
            // Configure authentication services
            services.AddScoped<IDomainUserGroupService, DomainUserGroupService>();
            services.AddScoped<IClaimsTransformation, CustomClaimsTransform>();
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<ILdapAuthenticationService, LdapAuthenticationService>();
            services.AddScoped<RoleUpdateRequest>();

            // Cookies stored key ring for secure storage (switch to E:\ for Local Development)
            if (config["EnvironmentLoaded"]?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
            {
                services.AddDataProtection()
               .PersistKeysToFileSystem(new DirectoryInfo(@"E:\ApplicationName\TOKENS"))
               .SetApplicationName(config["CookieName"]!);
            }
            else
            {
                services.AddDataProtection()
               .PersistKeysToDbContext<AppDbContext>()
               .SetApplicationName(config["CookieName"]!);
            }

            services.AddSingleton<ISessionStore>(provider =>
            {
                var distributedCache = provider.GetRequiredService<IDistributedCache>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new DistributedSessionStore(distributedCache, loggerFactory);
            });

            // Configure session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Persisted Cookies for Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = config["CookieName"];
                    options.Cookie.Domain = config["DomainSite"];
                    options.Cookie.Path = "/";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.LoginPath = "/account/cookielogin";
                    options.ExpireTimeSpan = TimeSpan.FromHours(config.GetValue<int>("SessionTimeout", 1));
                    options.SlidingExpiration = true;
                });

            // Uses the Authentication Cookie as a Handler for Post Requests to other Applications
            services.AddTransient<CookieHandler>();
            services.AddHttpClient(config["CookieName"]!)
                    .AddHttpMessageHandler<CookieHandler>();

            services.AddHttpClient("AuthenticationAPI", c =>
            {
                c.BaseAddress = new Uri(config["AuthenticationAPIURL"]!);
            })
            .AddHttpMessageHandler<CookieHandler>();
            #endregion

            #region Authorization Services
            // Authorization Services, Use LdapRoleMappingConfig to Build a list of Policies and Roles
            services.AddAuthorization(options =>
            {
                var ldapRoleMappingConfig = services.BuildServiceProvider().GetRequiredService<LdapRoleMappingConfig>();
                foreach (var key in ldapRoleMappingConfig.LdapRoleMappings.Keys)
                {
                    options.AddPolicy(key, policy => policy.RequireRole(key));
                }

                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole(ldapRoleMappingConfig.LdapRoleMappings.Keys.ToArray())
                    .Build();
            });
            #endregion
        }

        private static void StandardMiddleware(WebApplication app, IConfiguration config)
        {
            #region Middleware Configuration
            // Set the Base HREF to AppSettings.JSON Property. Set to where the application needs to be mounted on IIS Server.
            var appConfiguration = app.Services.GetRequiredService<IAppConfiguration>();
            var baseUrl = appConfiguration.BaseUrl;
            app.UsePathBase(baseUrl);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error", createScopeForErrors: true);
            }

            // Set up the middleware pipeline

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapControllers();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(typeof(RCL._Imports).Assembly);
            #endregion
        }
    }
}