using System;
using System.Threading.Tasks;
using MudBlazor;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Provides methods and properties to manage application themes.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the current theme.
        /// </summary>
        MudTheme CurrentTheme { get; }

        /// <summary>
        /// Gets a value indicating whether the current theme is light mode.
        /// </summary>
        bool IsLightMode { get; }

        /// <summary>
        /// Event triggered when the theme changes.
        /// </summary>
        event Action? OnChange;

        /// <summary>
        /// Initializes the theme based on user preferences or default settings.
        /// </summary>
        Task InitializeThemeAsync();

        /// <summary>
        /// Toggles the current theme between light and dark mode.
        /// </summary>
        Task ToggleThemeAsync();
    }
}