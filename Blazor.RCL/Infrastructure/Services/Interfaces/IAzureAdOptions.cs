using Blazor.RCL.Domain.Entities.AzureAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for accessing Azure AD configuration options.
    /// </summary>
    public interface IAzureAdOptions
    {
        /// <summary>
        /// Gets the client ID of the Azure AD application.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Gets the tenant ID of the Azure AD application.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Gets the object ID of the Azure AD application registration.
        /// </summary>
        string ApplicationRegistrationObjectId { get; }

        /// <summary>
        /// Gets the object ID of the Azure AD service principal.
        /// </summary>
        string ServicePrincipalObjectId { get; }

        /// <summary>
        /// Gets the registry key for the service principal.
        /// </summary>
        string ServicePrincipalRegistryKey { get; }

        /// <summary>
        /// Gets the registry key for the service principal.
        /// </summary>
        string ServicePrincipalAKeylessKey { get; }

        /// <summary>
        /// Gets the list of API permissions.
        /// </summary>
        IReadOnlyList<ApiPermission> ApiPermissions { get; }

        /// <summary>
        /// Gets the SharePoint site ID.
        /// </summary>
        string SharePointSiteId { get; }

        /// <summary>
        /// Gets the AKeyLess configuration.
        /// </summary>
        AKeyLessOptions AKeyLess { get; }
    }
}
