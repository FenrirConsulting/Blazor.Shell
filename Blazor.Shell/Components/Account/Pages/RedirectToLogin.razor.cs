using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Shell.Components.Account.Pages
{
    public partial class RedirectToLogin
    {
        [Inject] NavigationManager? NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager!.NavigateTo("/Account/AccessDenied", true);
        }
    }
}
