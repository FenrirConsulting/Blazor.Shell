using Blazor.RCL.Domain.Entities;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.Shell.Components.Pages
{
    /// <summary>
    /// Represents the remote access page component.
    /// </summary>
    public partial class RemoteAccess : ComponentBase
    {
        #region Injected Services
        [Inject] private ISnackbar? Snackbar { get; set; }
        [Inject] private RemoteScriptList RemoteScriptListService { get; set; } = null!;
        [Inject] private ServiceAccountList ServiceAccountListService { get; set; } = null!;
        #endregion

        #region Private Fields
        private Dictionary<string, string> _dynamicVariables = new Dictionary<string, string>();
        private string _consoleOutput = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of available service accounts.
        /// </summary>
        protected IReadOnlyList<ServiceAccount> ServiceAccountList => ServiceAccountListService.Accounts;

        /// <summary>
        /// Gets the list of available remote scripts.
        /// </summary>
        protected IReadOnlyList<RemoteScript> RemoteScriptList => RemoteScriptListService.Scripts;

        /// <summary>
        /// Gets or sets the selected service account.
        /// </summary>
        protected ServiceAccount? SelectedServiceAccount { get; set; }

        /// <summary>
        /// Gets or sets the selected remote script.
        /// </summary>
        protected RemoteScript? SelectedRemoteScript
        {
            get => _selectedRemoteScript;
            set
            {
                if (_selectedRemoteScript != value)
                {
                    _selectedRemoteScript = value;
                    OnRemoteScriptSelected(value);
                }
            }
        }
        private RemoteScript? _selectedRemoteScript;

        /// <summary>
        /// Gets or sets the dynamic variables for the selected script.
        /// </summary>
        protected Dictionary<string, string> DynamicVariables
        {
            get => _dynamicVariables;
            set
            {
                _dynamicVariables = value;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Gets or sets the console output.
        /// </summary>
        protected string ConsoleOutput
        {
            get => _consoleOutput;
            set
            {
                _consoleOutput = value;
                StateHasChanged();
            }
        }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAll(
                RemoteScriptListService.ReloadScriptsAsync(),
                ServiceAccountListService.ReloadAccountsAsync()
            );
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the selection of a remote script.
        /// </summary>
        /// <param name="value">The selected remote script.</param>
        private void OnRemoteScriptSelected(RemoteScript? value)
        {
            if (value != null && !string.IsNullOrEmpty(value.Variables))
            {
                try
                {
                    DynamicVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(value.Variables) ?? new Dictionary<string, string>();
                }
                catch (JsonException)
                {
                    Snackbar?.Add("Error parsing variables JSON", Severity.Error);
                    DynamicVariables = new Dictionary<string, string>();
                }
            }
            else
            {
                DynamicVariables = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Executes the selected script.
        /// </summary>
        protected void ExecuteScript()
        {
            if (SelectedServiceAccount == null || SelectedRemoteScript == null)
            {
                Snackbar?.Add("Please select both a Service Account and a Remote Script", Severity.Warning);
                return;
            }

            var outputBuilder = new StringBuilder();

            // First line: ServiceAccount information
            outputBuilder.AppendLine($"{SelectedServiceAccount.Domain}\\{SelectedServiceAccount.SAM}");

            // Second line: Dynamic Variables
            outputBuilder.AppendLine("Here are the passed Dynamic Values:");
            foreach (var variable in DynamicVariables)
            {
                outputBuilder.AppendLine($"  {variable.Key}: {variable.Value}");
            }

            // Update ConsoleOutput
            ConsoleOutput = outputBuilder.ToString();

            // TODO: Implement actual script execution logic here

            Snackbar?.Add("Script execution started", Severity.Info);
        }
        #endregion
    }
}