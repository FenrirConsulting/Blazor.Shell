using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components
{
    public partial class ScriptVariablesEditor : ComponentBase
    {
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private List<ScriptVariable> Variables { get; set; } = new();

        protected override void OnInitialized()
        {
            InitializeVariables();
        }

        protected override void OnParametersSet()
        {
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            try
            {
                if (string.IsNullOrEmpty(Value))
                {
                    Variables = new List<ScriptVariable>();
                    return;
                }

                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(Value);
                if (dict != null)
                {
                    Variables = dict.Select(kv => new ScriptVariable
                    {
                        Name = kv.Key,
                        Value = kv.Value
                    }).ToList();
                }
                else
                {
                    Variables = new List<ScriptVariable>();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error initializing variables: {ex.Message}", Severity.Warning);
                Variables = new List<ScriptVariable>();
            }
        }

        private async Task NotifyChange()
        {
            try
            {
                var dict = Variables
                    .Where(v => !string.IsNullOrWhiteSpace(v.Name))
                    .ToDictionary(v => v.Name, v => v.Value);
                var json = JsonSerializer.Serialize(dict);
                await ValueChanged.InvokeAsync(json);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error updating variables: {ex.Message}", Severity.Error);
            }
        }

        private async Task HandleAddVariable()
        {
            try
            {
                Variables.Add(new ScriptVariable { Name = $"NewVar{Variables.Count + 1}", Value = "" });
                await NotifyChange();
                StateHasChanged();
                Snackbar.Add($"Added new variable. Total count: {Variables.Count}", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error in HandleAddVariable: {ex.Message}", Severity.Error);
            }
        }

        private async Task RemoveVariable(ScriptVariable variable)
        {
            try
            {
                Variables.Remove(variable);
                await NotifyChange();
                StateHasChanged();
                Snackbar.Add($"Removed variable. Remaining count: {Variables.Count}", Severity.Info);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error removing variable: {ex.Message}", Severity.Error);
            }
        }

        private class ScriptVariable
        {
            public string Name { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }
    }
}