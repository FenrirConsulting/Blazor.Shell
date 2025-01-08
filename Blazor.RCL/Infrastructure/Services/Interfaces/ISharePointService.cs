using Microsoft.Graph.Models;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for interacting with SharePoint through Microsoft Graph API.
    /// </summary>
    public interface ISharePointService
    {
        /// <summary>
        /// Retrieves documents from a specified SharePoint document library.
        /// </summary>
        /// <param name="documentLibraryName">Name of the document library.</param>
        /// <param name="folderPath">Optional path to a specific folder within the library.</param>
        /// <returns>A list of DriveItem representing the documents.</returns>
        Task<List<DriveItem>> GetDocumentsAsync(string documentLibraryName, string? folderPath = null);

        /// <summary>
        /// Downloads the content of a specific document from SharePoint.
        /// </summary>
        /// <param name="documentId">The ID of the document to download.</param>
        /// <returns>A Stream containing the document's content, or null if the content couldn't be retrieved.</returns>
        Task<Stream?> DownloadDocumentAsync(string documentId);
    }
}