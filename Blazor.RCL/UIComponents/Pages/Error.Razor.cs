using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Pages
{
    /// <summary>
    /// Represents the Error page component.
    /// </summary>
    public partial class Error : ComponentBase
    {
        #region Injected Services
        /// <summary>
        /// Service for logging.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        #region Parameters
        /// <summary>
        /// The HttpContext for the current request.
        /// </summary>
        [CascadingParameter] private HttpContext? HttpContext { get; set; }
        #endregion

        #region Private Properties
        /// <summary>
        /// The ID of the current request.
        /// </summary>
        private string? RequestId { get; set; }

        /// <summary>
        /// Indicates whether to show the RequestId.
        /// </summary>
        private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and sets the RequestId.
        /// </summary>
        protected override void OnInitialized()
        {
            try
            {
                RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
                _logger.LogMessage($"Error page initialized. RequestId: {RequestId}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred during Error page initialization", ex);
            }
        }
        #endregion
    }
}