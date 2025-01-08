using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Graph.Models;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for interacting with SharePoint through Microsoft Graph API.
    /// </summary>
    public class SharePointService : ISharePointService
    {
        #region Fields

        private readonly IConfidentialClientApplication _app;
        private readonly string[] _scopes = new[] { "https://graph.microsoft.com/.default" };
        private readonly string _fullSiteId;
        private readonly ILogHelper _logger;
        private string? _currentDriveId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SharePointService class.
        /// </summary>
        /// <param name="options">Azure AD options.</param>
        /// <param name="logger">Logger instance.</param>
        public SharePointService(IAzureAdOptions options, ILogHelper logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                _app = ConfidentialClientApplicationBuilder
                    .Create(options.ClientId)
                    .WithClientSecret(options.ServicePrincipalRegistryKey)
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{options.TenantId}/v2.0"))
                    .Build();

                _fullSiteId = options.SharePointSiteId ?? throw new ArgumentNullException(nameof(options.SharePointSiteId));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing SharePointService", ex);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets an authenticated GraphServiceClient.
        /// </summary>
        /// <returns>An instance of GraphServiceClient.</returns>
        private async Task<GraphServiceClient> GetGraphClientAsync()
        {
            try
            {
                var authResult = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                return new GraphServiceClient(httpClient);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while acquiring Graph client", ex);
                throw;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves documents from a specified SharePoint document library.
        /// </summary>
        /// <param name="documentLibraryName">Name of the document library.</param>
        /// <param name="folderPath">Optional path to a specific folder within the library.</param>
        /// <returns>A list of DriveItem representing the documents.</returns>
        public async Task<List<DriveItem>> GetDocumentsAsync(string documentLibraryName, string? folderPath = null)
        {
            try
            {
                var graphClient = await GetGraphClientAsync();

                // Try to get the drive by name
                var drives = await graphClient.Sites[_fullSiteId].Drives
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = $"name eq '{documentLibraryName}'";
                    });

                if (drives?.Value == null || !drives.Value.Any())
                {
                    throw new Exception($"Drive '{documentLibraryName}' not found.");
                }

                var drive = drives.Value.First();
                _currentDriveId = drive.Id;

                DriveItem folderItem;
                if (string.IsNullOrEmpty(folderPath))
                {
                    folderItem = await graphClient.Drives[drive.Id].Root.GetAsync() ?? throw new Exception("Root folder not found.");
                }
                else
                {
                    folderItem = await graphClient.Drives[drive.Id].Root.ItemWithPath(folderPath).GetAsync() ?? throw new Exception($"Folder '{folderPath}' not found.");
                }

                var items = await graphClient.Drives[drive.Id].Items[folderItem.Id].Children
                    .GetAsync();

                return items?.Value?.ToList() ?? new List<DriveItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetDocumentsAsync: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Downloads the content of a specific document from SharePoint.
        /// </summary>
        /// <param name="documentId">The ID of the document to download.</param>
        /// <returns>A Stream containing the document's content, or null if the content couldn't be retrieved.</returns>
        public async Task<Stream?> DownloadDocumentAsync(string documentId)
        {
            try
            {
                var graphClient = await GetGraphClientAsync();

                if (string.IsNullOrEmpty(_currentDriveId))
                {
                    throw new InvalidOperationException("Drive ID is not set. Ensure GetDocumentsAsync is called before DownloadDocumentAsync.");
                }

                var stream = await graphClient.Drives[_currentDriveId].Items[documentId].Content
                    .GetAsync();

                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DownloadDocumentAsync: {ex.Message}", ex);
                return null;
            }
        }

        #endregion
    }
}