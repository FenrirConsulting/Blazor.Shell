using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.Infrastructure.Themes;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for managing theme settings and dark mode preferences.
    /// </summary>
    public class ThemeService : IThemeService
    {
        #region Fields

        private readonly IUserSettingsService _userSettingsService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ThemeService class.
        /// </summary>
        /// <param name="userSettingsService">Service for managing user settings.</param>
        /// <param name="authenticationStateProvider">Provider for authentication state.</param>
        /// <param name="logger">Logging service.</param>
        public ThemeService(
            IUserSettingsService userSettingsService,
            AuthenticationStateProvider authenticationStateProvider,
            ILogHelper logger)
        {
            _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
            _authenticationStateProvider = authenticationStateProvider ?? throw new ArgumentNullException(nameof(authenticationStateProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        public MudTheme CurrentTheme { get; private set; } = PaletteThemes.LightCompanyTheme;

        /// <summary>
        /// Gets a value indicating whether the current theme is light mode.
        /// </summary>
        public bool IsLightMode { get; private set; } = true;

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the theme changes.
        /// </summary>
        public event Action? OnChange;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the theme based on user preferences or default settings.
        /// </summary>
        public async Task InitializeThemeAsync()
        {
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    var isDarkMode = await _userSettingsService.GetIsDarkModeAsync(user);
                    SetThemeBasedOnUserPreference(isDarkMode);
                }
                else
                {
                    CurrentTheme = PaletteThemes.LightCompanyTheme;
                    IsLightMode = true;
                }
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while initializing the theme.", ex);
            }
        }

        /// <summary>
        /// Toggles the current theme between light and dark mode.
        /// </summary>
        public async Task ToggleThemeAsync()
        {
            try
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity?.IsAuthenticated == true)
                {
                    var currentDarkMode = await _userSettingsService.GetIsDarkModeAsync(user);
                    var newDarkMode = !currentDarkMode;
                    await _userSettingsService.SetIsDarkModeAsync(user, newDarkMode);
                    SetThemeBasedOnUserPreference(newDarkMode);
                }
                else
                {
                    CurrentTheme = CurrentTheme == PaletteThemes.LightCompanyTheme ? PaletteThemes.DarkModernTheme : PaletteThemes.LightCompanyTheme;
                    IsLightMode = CurrentTheme == PaletteThemes.LightCompanyTheme;
                }
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while toggling the theme.", ex);
            }
        }

        #endregion

        #region Private Methods

        private void SetThemeBasedOnUserPreference(bool isDarkMode)
        {
            CurrentTheme = isDarkMode ? PaletteThemes.DarkModernTheme : PaletteThemes.LightCompanyTheme;
            IsLightMode = !isDarkMode;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        #endregion
    }
}