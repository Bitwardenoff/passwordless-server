using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Passwordless.AdminConsole.EventLog.DTOs;
using Passwordless.AdminConsole.Middleware;
using Passwordless.AdminConsole.Services.PasswordlessManagement;
using Passwordless.Common.Models.Apps;
using Passwordless.Common.Models.MDS;
using Passwordless.Common.Models.Reporting;

namespace Passwordless.AdminConsole.Services;

public interface IScopedPasswordlessClient : IPasswordlessClient
{
    Task<ApplicationEventLogResponse> GetApplicationEventLog(int pageNumber, int pageSize);
    Task<IEnumerable<PeriodicCredentialReportResponse>> GetPeriodicCredentialReportsAsync(PeriodicCredentialReportRequest request);
    Task<IEnumerable<string>> GetAttestationTypesAsync();
    Task<IEnumerable<string>> GetCertificationStatusesAsync();
    Task<IEnumerable<EntryResponse>> GetMetaDataStatementEntriesAsync(EntriesRequest request);
    Task<IEnumerable<ConfiguredAuthenticatorResponse>> GetConfiguredAuthenticatorsAsync(ConfiguredAuthenticatorRequest request);
    Task WhitelistAuthenticatorsAsync(WhitelistAuthenticatorsRequest request);
    Task DelistAuthenticatorsAsync(DelistAuthenticatorsRequest request);
}

public class ScopedPasswordlessClient : PasswordlessClient, IScopedPasswordlessClient
{
    private readonly HttpClient _client;
    private readonly ICurrentContext _currentContext;

    public ScopedPasswordlessClient(
        HttpClient httpClient,
        IOptions<PasswordlessManagementOptions> options,
        ICurrentContext context)
        : base(new PasswordlessOptions
        {
            ApiSecret = context.ApiSecret!,
            ApiUrl = options.Value.InternalApiUrl,
        })
    {
        _client = httpClient;
        _currentContext = context;

        // can be dropped when call below is moved to the SDK.
        _client.DefaultRequestHeaders.Remove("ApiSecret");
        _client.DefaultRequestHeaders.Add("ApiSecret", context.ApiSecret);
    }

    public async Task<ApplicationEventLogResponse> GetApplicationEventLog(int pageNumber, int pageSize)
    {
        var response = await _client.GetAsync($"events?pageNumber={pageNumber}&numberOfResults={pageSize}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ApplicationEventLogResponse>())!;
    }

    public async Task<IEnumerable<PeriodicCredentialReportResponse>> GetPeriodicCredentialReportsAsync(PeriodicCredentialReportRequest request)
    {
        var queryBuilder = new QueryBuilder();
        if (request.From.HasValue)
        {
            queryBuilder.Add("from", request.From.Value.ToString("yyyy-MM-dd"));
        }
        if (request.To.HasValue)
        {
            queryBuilder.Add("to", request.To.Value.ToString("yyyy-MM-dd"));
        }

        var q = queryBuilder.ToQueryString();
        var response = await _client.GetAsync($"/apps/{_currentContext.AppId}/reporting/credentials/periodic{q}");
        response.EnsureSuccessStatusCode();

        var rest = (await response.Content.ReadFromJsonAsync<IEnumerable<PeriodicCredentialReportResponse>>())!;
        return rest;
    }

    public async Task<IEnumerable<string>> GetAttestationTypesAsync()
    {
        var response = await _client.GetAsync("/mds/attestation-types");
        return (await response.Content.ReadFromJsonAsync<IEnumerable<string>>())!;
    }

    public async Task<IEnumerable<string>> GetCertificationStatusesAsync()
    {
        var response = await _client.GetAsync("/mds/certification-statuses");
        return (await response.Content.ReadFromJsonAsync<IEnumerable<string>>())!;
    }

    public async Task<IEnumerable<EntryResponse>> GetMetaDataStatementEntriesAsync(EntriesRequest request)
    {
        var queryBuilder = new QueryBuilder();
        if (request.AttestationTypes != null)
        {
            foreach (var attestationType in request.AttestationTypes)
            {
                queryBuilder.Add(nameof(request.AttestationTypes), attestationType);
            }
        }
        if (request.CertificationStatuses != null)
        {
            foreach (var certificationStatus in request.CertificationStatuses)
            {
                queryBuilder.Add(nameof(request.CertificationStatuses), certificationStatus);
            }
        }
        var q = queryBuilder.ToQueryString();
        return (await _client.GetFromJsonAsync<EntryResponse[]>($"/mds/entries{q}"))!;
    }

    public async Task<IEnumerable<ConfiguredAuthenticatorResponse>> GetConfiguredAuthenticatorsAsync(ConfiguredAuthenticatorRequest request)
    {
        var queryBuilder = new QueryBuilder();
        queryBuilder.Add(nameof(request.IsAllowed), request.IsAllowed.ToString());
        var q = queryBuilder.ToQueryString();
        return (await _client.GetFromJsonAsync<ConfiguredAuthenticatorResponse[]>($"/apps/list-authenticators{q}"))!;
    }

    public async Task WhitelistAuthenticatorsAsync(WhitelistAuthenticatorsRequest request)
    {
        var response = await _client.PostAsJsonAsync("/apps/whitelist-authenticators", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DelistAuthenticatorsAsync(DelistAuthenticatorsRequest request)
    {
        var response = await _client.PostAsJsonAsync("/apps/delist-authenticators", request);
        response.EnsureSuccessStatusCode();
    }
}