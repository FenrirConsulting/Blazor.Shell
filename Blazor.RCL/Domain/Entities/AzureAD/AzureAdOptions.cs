using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Domain.Entities.AzureAD
{
    /// <summary>
    /// Represents the Azure AD configuration options.
    /// </summary>
    public class AzureAdOptions
    {
        /// <summary>
        /// Gets or sets the client ID of the Azure AD application.
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the tenant ID of the Azure AD application.
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the object ID of the Azure AD application registration.
        /// </summary>
        public string? ApplicationRegistrationObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object ID of the Azure AD service principal.
        /// </summary>
        public string? ServicePrincipalObjectId { get; set; }

        /// <summary>
        /// Gets or sets the registry key for the service principal.
        /// </summary>
        public string? ServicePrincipalRegistryKey { get; set; }

        /// <summary>
        /// Gets the registry key for the service principal.
        /// </summary>
        public string? ServicePrincipalAKeylessKey { get; set; }

        /// <summary>
        /// Gets or sets the list of API permissions.
        /// </summary>
        public List<ApiPermission>? ApiPermissions { get; set; }

        /// <summary>
        /// Gets or sets the SharePoint site ID.
        /// </summary>
        public string? SharePointSiteId { get; set; }
    }
}
