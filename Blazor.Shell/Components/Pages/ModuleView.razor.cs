using Microsoft.AspNetCore.Components;
using Blazor.RCL.Infrastructure.Common.Configuration;
using MudBlazor;

namespace Blazor.Shell.Components.Pages
{
    /// <summary>
    /// Represents the module view component for displaying external modules.
    /// </summary>
    public partial class ModuleView : ComponentBase
    {
        #region Parameters
        [Parameter] public string? ModuleName { get; set; }
        #endregion

        #region Injected Services
        [Inject] private ISnackbar? _snackbar { get; set; }
        [Inject] private ViewModuleList? _moduleList { get; set; }
        [Inject] private NavigationManager? _navigationManager { get; set; }
        [Inject] private IWebHostEnvironment? _environment { get; set; }
        #endregion

        #region Private Fields
        private string? _moduleUrl = string.Empty;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Called when the component parameters are set.
        /// </summary>
        protected override void OnParametersSet()
        {
            UpdateModuleUrl();
            
            Console.WriteLine(_moduleUrl);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the module URL based on the current environment and module name.
        /// </summary>
        private void UpdateModuleUrl()
        {
            if (!string.IsNullOrEmpty(ModuleName))
            {
                bool isDevelopment = _environment!.IsDevelopment();
                var moduleUrl = _moduleList!.GetModuleUrl(ModuleName, isDevelopment);
                _moduleUrl = isDevelopment ? moduleUrl : $"{_navigationManager!.BaseUri.TrimEnd('/')}{moduleUrl}";
                StateHasChanged();
            }
        }
        #endregion
    }
}