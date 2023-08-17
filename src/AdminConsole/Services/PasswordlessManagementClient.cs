using Passwordless.AdminConsole.Models.DTOs;
using Passwordless.AdminConsole.Services.PasswordlessManagement.Contracts;
using Passwordless.Api.Models;

namespace Passwordless.AdminConsole.Services;

public class PasswordlessManagementClient : IPasswordlessManagementClient
{
    private readonly HttpClient _client;

    public PasswordlessManagementClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<NewAppResponse> CreateApplication(string appId, NewAppOptions registerOptions)
    {
        var res = await _client.PostAsJsonAsync($"/admin/apps/{appId}/create", registerOptions);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<NewAppResponse>();
    }

    public async Task<MarkDeleteApplicationResponse> MarkDeleteApplication(MarkDeleteApplicationRequest request)
    {
        var response = await _client.PostAsJsonAsync("apps/mark-delete", new { request.AppId, request.DeletedBy });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MarkDeleteApplicationResponse>();
    }

    public async Task<ICollection<string>> ListApplicationsPendingDeletionAsync()
    {
        var response = await _client.GetAsync("apps/list-pending-deletion");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ICollection<string>>();
    }

    public async Task<bool> DeleteApplicationAsync(string application)
    {
        var request = new { appId = application };
        var res = await _client.PostAsJsonAsync("apps/delete", request);
        var why = await res.Content.ReadAsStringAsync();
        return res.IsSuccessStatusCode;
    }

    public async Task<CancelApplicationDeletionResponse> CancelApplicationDeletion(string applicationId)
    {
        var response = await _client.GetAsync($"apps/delete/cancel/{applicationId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CancelApplicationDeletionResponse>();
    }

    public async Task SetFeaturesAsync(SetApplicationFeaturesRequest request)
    {
        var response = await _client.PostAsJsonAsync($"apps/features", request);
        response.EnsureSuccessStatusCode();
    }
}