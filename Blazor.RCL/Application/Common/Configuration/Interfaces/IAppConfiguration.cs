namespace Blazor.RCL.Application.Common.Configuration.Interfaces
{
    /// <summary>
    /// Defines the contract for application configuration settings.
    /// </summary>
    public interface IAppConfiguration
    {
        #region Application Information

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        string AppName { get; }

        /// <summary>
        /// Gets the label of the application.
        /// </summary>
        string AppLabel { get; }

        /// <summary>
        /// Gets the environment in which the application is loaded.
        /// </summary>
        string EnvironmentLoaded { get; }

        #endregion

        #region URLs and Endpoints

        /// <summary>
        /// Gets the base URL of the application.
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Gets the domain site URL.
        /// </summary>
        string DomainSite { get; }

        /// <summary>
        /// Gets the URL for the authentication API.
        /// </summary>
        string AuthenticationAPIURL { get; }

        /// <summary>
        /// Gets the return URL after authentication.
        /// </summary>
        string ReturnURL { get; }

        #endregion

        #region Database and Storage

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Additional dynamic connection strings.
        /// </summary>
        Dictionary<string, string> AdditionalConnectionStrings { get; }

        /// <summary>
        /// Gets a dictionary of registry values.
        /// </summary>
        Dictionary<string, string> RegistryValues { get; }

        /// <summary>
        /// Gets a dictionary of dynamic values.
        /// </summary>
        Dictionary<string, string> DynamicValues { get; }

        #endregion

        #region Authentication and Security

        /// <summary>
        /// Gets the name of the authentication cookie.
        /// </summary>
        string CookieName { get; }

        /// <summary>
        /// Gets the timeout duration for the authentication cookie in minutes.
        /// </summary>
        double SessionTimeout { get; }
        #endregion
    }
}