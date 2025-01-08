using System.Collections.Generic;

namespace Blazor.RCL.Domain.Entities
{
    /// <summary>
    /// Represents a collection of server host configurations.
    /// </summary>
    public class ServerHostConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list of server hosts.
        /// </summary>
        public List<ServerHost> Hosts { get; set; } = new();

        #endregion
    }
}
