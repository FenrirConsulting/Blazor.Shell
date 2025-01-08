using Microsoft.AspNetCore.Components;
using Blazor.RCL.NLog.LogService.Interface;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components
{
    public partial class JsonDialog : ComponentBase, IDisposable
    {
        #region Parameters and Injected Services
        /// <summary>
        /// Event callback for toggling the sidebar.
        /// </summary>
        [Parameter] public string? JsonContent { get; set; }

        /// <summary>
        /// Application configuration service.
        /// </summary>
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;

        /// <summary>
        /// Logging service.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        private void CloseDialog()
        {
            MudDialog.Close();
        }
        /// <summary>
        /// Disposes the resources used by the AppBar component.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Currently, there are no resources to dispose.
                // This method is implemented for future-proofing and consistency.
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while disposing AppBar", ex);
            }
        }
    }
}
