using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Blazor.RCL.Application.Models;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Manages a list of view modules for the NavMenu lookup.
    /// </summary>
    public class ViewModuleList
    {
        #region Fields

        private readonly IConfiguration _configuration;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dictionary of view modules.
        /// </summary>
        public Dictionary<string, ViewModule> Modules { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ViewModuleList class.
        /// </summary>
        /// <param name="configuration">The configuration to bind view modules from.</param>
        public ViewModuleList(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Modules = new Dictionary<string, ViewModule>();
            InitializeModules();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the URL for a specified module based on the development environment.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="isDevelopment">Flag indicating whether the environment is development.</param>
        /// <returns>The URL of the module, or null if not found.</returns>
        public string? GetModuleUrl(string moduleName, bool isDevelopment)
        {
            if (Modules.TryGetValue(moduleName, out var moduleInfo))
            {
                return isDevelopment ? moduleInfo.DevelopmentUri : moduleInfo.Url;
            }
            return null;
        }

        #endregion

        #region Private Methods

        private void InitializeModules()
        {
            _configuration.GetSection("ViewModules").Bind(Modules);
        }

        #endregion
    }
}