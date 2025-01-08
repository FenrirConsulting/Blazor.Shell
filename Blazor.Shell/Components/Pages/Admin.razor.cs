using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Infrastructure.Services.Interfaces;

namespace Blazor.Shell.Components.Pages
{
    public partial class Admin : ComponentBase
    {
        #region Injected Services
        [Inject] private IToolsConfigurationRepository? _toolsRepository { get; set; }
        [Inject] private IServerHostRepository? _serverHostRepository { get; set; }
        [Inject] private IServiceAccountRepository? _serviceAccountRepository { get; set; }
        [Inject] private IRemoteScriptRepository? _remoteScriptRepository { get; set; }
        [Inject] private ILDAPServerRepository? _ldapServerRepository { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private ISnackbar? _snackbar { get; set; }
        [Inject] private ILogHelper? _logger { get; set; }
        [Inject] private IRequestRefresh? _requestRefresh { get; set; }
        [Inject] private LdapServerList? _ldapServerList { get; set; }
        [Inject] private ServiceAccountList? _serviceAccountList { get; set; }
        [Inject] private RemoteScriptList? _remoteScriptList { get; set; }
        [Inject] private NavLinksInfoList? _navLinksInfoList { get; set; }
        [Inject] private ServerHostList? _serverHostList { get; set; }
        #endregion

        #region Private Fields
        private List<ToolsConfiguration>? _toolConfigurations;
        private List<ToolsConfiguration>? _authConfigurations;
        private List<ServiceAccount>? _serviceAccounts;
        private List<LDAPServer>? _ldapServers;
        
        private object? _editingItem;
        private bool _isDialogVisible;
        private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        private EditingType _currentEditingType;
        private bool _isNewItem;

        // Server Host Fields
        private List<ServerHost>? _serverHosts;
        private ServerHost? _editingHost;
        private bool _isHostDialogVisible;
        private bool _isNewHost;
        private string? _currentUser;

        // Remote Script Fields
        private List<RemoteScript>? _remoteScripts;
        private RemoteScript? _editingScript;
        private bool _isScriptDialogVisible;
        private bool _isNewScript;
        #endregion

        #region Enums
        private enum EditingType
        {
            ToolsConfiguration,
            ServiceAccount,
            RemoteScript,
            LDAPServer
        }
        #endregion

        #region Lifecycle Methods
        protected override async Task OnInitializedAsync()
        {
            await LoadAllData();
            await LoadServerHosts();
            await LoadRemoteScripts();
        }
        #endregion

        #region Private Methods
        private async Task LoadAllData()
        {
            try
            {
                var tasks = new List<Task>
                {
                    LoadToolConfigurations(),
                    LoadServiceAccounts(),
                    LoadLDAPServers()
                };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _snackbar!.Add("Error fetching data", Severity.Error);
                _logger!.LogError("Error fetching data", ex);
            }
        }

        private async Task LoadToolConfigurations()
        {
            var configurations = await _toolsRepository!.GetAllByApplicationAsync(_appConfiguration!.AppName);
            var authConfigurations = await _toolsRepository!.GetAllByApplicationAsync("AuthenticateAPI");
            _toolConfigurations = configurations.ToList();
            _authConfigurations = authConfigurations.ToList();
        }

        private async Task LoadServiceAccounts()
        {
            _serviceAccounts = (await _serviceAccountRepository!.GetAllAsync()).ToList();
        }

        private async Task LoadLDAPServers()
        {
            _ldapServers = (await _ldapServerRepository!.GetAllAsync()).ToList();
        }

        private void OpenEditor(object? item, EditingType type)
        {
            _currentEditingType = type;
            _isNewItem = item == null;

            if (_isNewItem)
            {
                _editingItem = type switch
                {
                    EditingType.ToolsConfiguration => new ToolsConfiguration { Application = _appConfiguration!.AppName },
                    EditingType.ServiceAccount => new ServiceAccount(),
                    EditingType.LDAPServer => new LDAPServer(),
                    _ => throw new ArgumentException("Invalid editing type")
                };
            }
            else
            {
                _editingItem = item;
            }

            _isDialogVisible = true;
        }

        private void CancelEdit()
        {
            _editingItem = null;
            _isDialogVisible = false;
            _isNewItem = false;
        }

        private async Task SaveChanges()
        {
            try
            {
                switch (_currentEditingType)
                {
                    case EditingType.ToolsConfiguration:
                        var config = (ToolsConfiguration)_editingItem!;
                        await _toolsRepository!.SetAsync(config);
                        await _navLinksInfoList!.ReloadLinksAsync();
                        break;
                    case EditingType.ServiceAccount:
                        var account = (ServiceAccount)_editingItem!;
                        if (_isNewItem)
                        {
                            await _serviceAccountRepository!.AddAsync(account);
                            await _serviceAccountList!.ReloadAccountsAsync();
                        }
                        else
                        {
                            await _serviceAccountRepository!.UpdateAsync(account);
                            await _serviceAccountList!.ReloadAccountsAsync();
                        }
                        break;
                    case EditingType.LDAPServer:
                        var server = (LDAPServer)_editingItem!;
                        if (_isNewItem)
                        {
                            await _ldapServerRepository!.AddAsync(server);
                            await _ldapServerList!.ReloadServersAsync();
                        }
                        else
                        {
                            await _ldapServerRepository!.UpdateAsync(server);
                            await _ldapServerList!.ReloadServersAsync();
                        }
                        break;
                }
                _requestRefresh!.CallRequestRefresh();

                _snackbar!.Add(_isNewItem ? "Item added successfully" : "Item updated successfully", Severity.Success);
                await LoadAllData(); // Refresh all data
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error {(_isNewItem ? "adding" : "updating")} item: {ex.Message}", Severity.Error);
                _logger!.LogError($"Error {(_isNewItem ? "adding" : "updating")} item:", ex);
            }
            finally
            {
                _editingItem = null;
                _isDialogVisible = false;
                _isNewItem = false;
            }
        }

        private async Task RemoveItem(long id, EditingType type)
        {
            try
            {
                switch (type)
                {
                    case EditingType.ToolsConfiguration:
                        await _toolsRepository!.RemoveAsync(new ToolsConfiguration { Id = id });
                        break;
                    case EditingType.ServiceAccount:
                        await _serviceAccountRepository!.RemoveAsync(id);
                        break;
                    case EditingType.LDAPServer:
                        await _ldapServerRepository!.RemoveAsync(id);
                        break;
                    default:
                        throw new ArgumentException("Invalid item type for removal");
                }

                _snackbar!.Add("Item removed successfully", Severity.Success);
                await LoadAllData(); // Refresh all data
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error removing item: {ex.Message}", Severity.Error);
                _logger!.LogError("Error removing item:", ex);
            }
        }
        #endregion

        #region Server Host Methods
        /// <summary>
        /// Loads server hosts from the repository.
        /// </summary>
        private async Task LoadServerHosts()
        {
            try
            {
                var hosts = await _serverHostRepository!.GetAllAsync();
                _serverHosts = hosts.ToList();
            }
            catch (Exception ex)
            {
                _snackbar!.Add("Error loading server hosts", Severity.Error);
                _logger!.LogError("Error loading server hosts", ex);
            }
        }

        /// <summary>
        /// Opens the host editor dialog.
        /// </summary>
        private void OpenHostEditor(ServerHost? host)
        {
            _isNewHost = host == null;
            _editingHost = _isNewHost
                ? new ServerHost { AuditAddBy = _currentUser ?? "Unknown" }
                : new ServerHost
                {
                    Id = host.Id,
                    Hostname = host.Hostname,
                    Environment = host.Environment,
                    Name = host.Name,
                    Role = host.Role,
                    AuditChangeBy = _currentUser,
                    AuditChangeDate = DateTime.Now,
                    AuditAddBy = host.AuditAddBy,
                    AuditAddDate = host.AuditAddDate
                };
            _isHostDialogVisible = true;
        }

        /// <summary>
        /// Cancels the host editing operation.
        /// </summary>
        private void CancelHostEdit()
        {
            _editingHost = null;
            _isHostDialogVisible = false;
            _isNewHost = false;
        }

        /// <summary>
        /// Saves changes to a server host.
        /// </summary>
        private async Task SaveHostChanges()
        {
            try
            {
                if (_isNewHost)
                {
                    _editingHost!.AuditAddDate = DateTime.Now;
                    await _serverHostRepository!.AddAsync(_editingHost);
                }
                else
                {
                    await _serverHostRepository!.UpdateAsync(_editingHost!);
                }

                await _serverHostList!.ReloadHostsAsync();
                _requestRefresh!.CallRequestRefresh();
                _snackbar!.Add(_isNewHost ? "Host added successfully" : "Host updated successfully", Severity.Success);
                await LoadServerHosts();
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error {(_isNewHost ? "adding" : "updating")} host: {ex.Message}", Severity.Error);
                _logger!.LogError($"Error {(_isNewHost ? "adding" : "updating")} host:", ex);
            }
            finally
            {
                _editingHost = null;
                _isHostDialogVisible = false;
                _isNewHost = false;
            }
        }

        /// <summary>
        /// Removes a server host.
        /// </summary>
        private async Task RemoveHost(int id)
        {
            try
            {
                await _serverHostRepository!.RemoveAsync(id);
                await _serverHostList!.ReloadHostsAsync();
                _requestRefresh!.CallRequestRefresh();
                _snackbar!.Add("Host removed successfully", Severity.Success);
                await LoadServerHosts();
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error removing host: {ex.Message}", Severity.Error);
                _logger!.LogError("Error removing host:", ex);
            }
        }
        #endregion

        #region Remote Script Methods
        private async Task LoadRemoteScripts()
        {
            try
            {
                var scripts = await _remoteScriptRepository!.GetAllAsync();
                _remoteScripts = scripts.ToList();
            }
            catch (Exception ex)
            {
                _snackbar!.Add("Error loading remote scripts", Severity.Error);
                _logger!.LogError("Error loading remote scripts", ex);
            }
        }

        private void OpenScriptEditor(RemoteScript? script)
        {
            try
            {
                _isNewScript = script == null;
                _editingScript = _isNewScript
                    ? new RemoteScript
                    {
                        AuditAddBy = _currentUser ?? "Unknown",
                        AuditAddDate = DateTime.Now,
                        Variables = "{}", // Initialize with empty JSON object
                        Name = "",
                        Location = ""
                    }
                    : new RemoteScript
                    {
                        Id = script.Id,
                        Name = script.Name ?? "",
                        Location = script.Location ?? "",
                        Variables = string.IsNullOrEmpty(script.Variables) ? "{}" : script.Variables,
                        AuditAddBy = script.AuditAddBy,
                        AuditAddDate = script.AuditAddDate,
                        AuditChangeBy = _currentUser,
                        AuditChangeDate = DateTime.Now
                    };
                _isScriptDialogVisible = true;
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error in OpenScriptEditor: {ex.Message}", Severity.Error);
            }
        }

        private void CancelScriptEdit()
        {
            _editingScript = null;
            _isScriptDialogVisible = false;
            _isNewScript = false;
        }

        private async Task SaveScriptChanges()
        {
            try
            {
                if (_isNewScript)
                {
                    await _remoteScriptRepository!.AddAsync(_editingScript!);
                }
                else
                {
                    await _remoteScriptRepository!.UpdateAsync(_editingScript!);
                }

                await _remoteScriptList!.ReloadScriptsAsync();
                _requestRefresh!.CallRequestRefresh();
                _snackbar!.Add(_isNewScript ? "Script added successfully" : "Script updated successfully", Severity.Success);
                await LoadRemoteScripts();
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error {(_isNewScript ? "adding" : "updating")} script: {ex.Message}", Severity.Error);
                _logger!.LogError($"Error {(_isNewScript ? "adding" : "updating")} script:", ex);
            }
            finally
            {
                _editingScript = null;
                _isScriptDialogVisible = false;
                _isNewScript = false;
            }
        }

        private async Task RemoveScript(RemoteScript script)
        {
            try
            {
                await _remoteScriptRepository!.RemoveAsync(script.Id);
                await _remoteScriptList!.ReloadScriptsAsync();
                _requestRefresh!.CallRequestRefresh();
                _snackbar!.Add("Script removed successfully", Severity.Success);
                await LoadRemoteScripts();
            }
            catch (Exception ex)
            {
                _snackbar!.Add($"Error removing script: {ex.Message}", Severity.Error);
                _logger!.LogError("Error removing script:", ex);
            }
        }
        #endregion
    }
}