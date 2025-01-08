using Blazor.Shell;

#region Program
var builder = WebApplication.CreateBuilder(args);

// Use static web assets
builder.WebHost.UseStaticWebAssets();

// Add Application Services in Dependency Injection
#pragma warning disable CA1416
builder.Services.AddServerUI(builder.Configuration);

var app = builder.Build();

// Build Application Configuration from Dependency Injection
app.ConfigureServer(builder.Configuration);
#pragma warning restore CA1416

await app.RunAsync();
#endregion