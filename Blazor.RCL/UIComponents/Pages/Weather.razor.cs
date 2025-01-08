using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Pages
{
    /// <summary>
    /// Represents the Weather page component.
    /// </summary>
    public partial class Weather : ComponentBase
    {
        #region Injected Services
        /// <summary>
        /// Service for handling navigation.
        /// </summary>
        [Inject] private NavigationManager? _navigationManager { get; set; }

        /// <summary>
        /// Service for managing authentication state.
        /// </summary>
        [Inject] private AuthenticationStateProvider? _authenticationStateProvider { get; set; }

        /// <summary>
        /// Service for logging.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;
        #endregion

        #region Private Fields
        /// <summary>
        /// Array of weather forecasts.
        /// </summary>
        private WeatherForecast[]? _forecasts;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component, checks user authentication and authorization, and loads weather data.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await _authenticationStateProvider!.GetAuthenticationStateAsync();
                var user = authState.User;

                if (!user.Identity!.IsAuthenticated)
                {
                    // User is not authenticated, redirect to login page
                    _navigationManager!.NavigateTo("/");
                    return;
                }

                if (!user.IsInRole("User"))
                {
                    // User is authenticated but does not have the required role, redirect to an access denied page
                    _navigationManager!.NavigateTo("/account/accessdenied");
                    return;
                }

                await LoadWeatherDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred during Weather page initialization", ex);
                _navigationManager!.NavigateTo("/error");
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads simulated weather data.
        /// </summary>
        private async Task LoadWeatherDataAsync()
        {
            // Simulate asynchronous loading to demonstrate a loading indicator
            await Task.Delay(500);

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

            _forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToArray();

            StateHasChanged();
        }
        #endregion

        #region Private Classes
        /// <summary>
        /// Represents a weather forecast.
        /// </summary>
        private class WeatherForecast
        {
            public DateOnly Date { get; set; }
            public int TemperatureC { get; set; }
            public string? Summary { get; set; }
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
        #endregion
    }
}