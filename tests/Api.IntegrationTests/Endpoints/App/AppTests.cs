using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Passwordless.Api.IntegrationTests.Helpers;
using Passwordless.Api.IntegrationTests.Helpers.App;
using Passwordless.Common.Constants;
using Passwordless.Common.Extensions;
using Passwordless.Common.Models.Apps;
using Passwordless.Service.Models;
using Passwordless.Service.Storage.Ef;
using Xunit;
using Xunit.Abstractions;
using static Passwordless.Api.IntegrationTests.Helpers.App.CreateAppHelpers;
using SetFeaturesRequest = Passwordless.Common.Models.Apps.SetFeaturesRequest;

namespace Passwordless.Api.IntegrationTests.Endpoints.App;

public class AppTests : IClassFixture<PasswordlessApiFactory>, IDisposable
{
    private readonly PasswordlessApiFactory _apiFactory;
    private readonly HttpClient _client;

    public AppTests(ITestOutputHelper testOutput, PasswordlessApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _apiFactory.TestOutput = testOutput;
        _client = apiFactory.CreateClient().AddManagementKey();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("1")]
    public async Task I_cannot_create_an_account_with_an_invalid_name(string name)
    {
        // Act
        using var response = await _client.CreateApplicationAsync(name);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        content.Should().NotBeNull();
        content!.Title.Should().Be("accountName needs to be alphanumeric and start with a letter");
    }

    [Fact]
    public async Task I_can_create_an_account_with_a_valid_name()
    {
        // Arrange
        const string accountName = "anders";

        // Act
        using var response = await _client.CreateApplicationAsync(accountName);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<CreateAppResultDto>();
        content.Should().NotBeNull();
        content!.Message.Should().Be("Store keys safely. They will only be shown to you once.");
        content.ApiKey1.Should().NotBeNullOrWhiteSpace();
        content.ApiKey2.Should().NotBeNullOrWhiteSpace();
        content.ApiSecret1.Should().NotBeNullOrWhiteSpace();
        content.ApiSecret2.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task I_can_create_an_app_and_its_features_will_be_set_correctly()
    {
        // Arrange
        var name = GetApplicationName();

        // Act
        using var res = await _client.CreateApplicationAsync(name);

        // Assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _apiFactory.Services.CreateScope();

        var appFeature = await scope.ServiceProvider.GetRequiredService<ITenantStorageFactory>().Create(name).GetAppFeaturesAsync();

        appFeature.Should().NotBeNull();
        appFeature!.Tenant.Should().Be(name);
        appFeature.EventLoggingIsEnabled.Should().BeFalse();
        appFeature.EventLoggingRetentionPeriod.Should().Be(365);
        appFeature.DeveloperLoggingEndsAt.Should().BeNull();
    }

    [Fact]
    public async Task I_can_set_event_logging_retention_period()
    {
        // Arrange
        const int expectedEventLoggingRetentionPeriod = 30;
        var name = GetApplicationName();
        using var appCreateResponse = await _client.CreateApplicationAsync(name);
        var appCreateDto = await appCreateResponse.Content.ReadFromJsonAsync<CreateAppResultDto>();
        using var appHttpClient = _apiFactory.CreateClient().AddSecretKey(appCreateDto!.ApiSecret1);

        // Act
        using var setFeatureResponse = await appHttpClient.PostAsJsonAsync("/apps/features",
            new SetFeaturesRequest
            {
                PerformedBy = "a_user",
                EventLoggingRetentionPeriod = expectedEventLoggingRetentionPeriod
            });

        // Assert
        setFeatureResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var scope = _apiFactory.Services.CreateScope();

        var appFeature = await scope.ServiceProvider.GetRequiredService<ITenantStorageFactory>().Create(name).GetAppFeaturesAsync();
        appFeature.Should().NotBeNull();
        appFeature!.EventLoggingRetentionPeriod.Should().Be(expectedEventLoggingRetentionPeriod);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(91)]
    public async Task I_can_not_set_the_event_logging_retention_period_to_an_invalid_value(int invalidRetentionPeriod)
    {
        // Arrange
        var name = GetApplicationName();
        using var appCreateResponse = await _client.CreateApplicationAsync(name);
        var appCreateDto = await appCreateResponse.Content.ReadFromJsonAsync<CreateAppResultDto>();
        using var appHttpClient = _apiFactory.CreateClient().AddSecretKey(appCreateDto!.ApiSecret1);

        // Act
        using var setFeatureResponse = await appHttpClient.PostAsJsonAsync("/apps/features", new SetFeaturesRequest
        {
            PerformedBy = "a_user",
            EventLoggingRetentionPeriod = invalidRetentionPeriod
        });

        // Assert
        setFeatureResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await setFeatureResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("One or more validation errors occurred.");
    }

    [Fact]
    public async Task I_can_manage_an_apps_features()
    {
        // Arrange
        const int expectedEventLoggingRetentionPeriod = 30;

        var name = GetApplicationName();
        _ = await _client.CreateApplicationAsync(name);
        var manageFeatureRequest = new ManageFeaturesRequest { EventLoggingRetentionPeriod = expectedEventLoggingRetentionPeriod, EventLoggingIsEnabled = true };

        // Act
        var manageFeatureResponse = await _client.PostAsJsonAsync($"/admin/apps/{name}/features", manageFeatureRequest);

        // Assert
        manageFeatureResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var scope = _apiFactory.Services.CreateScope();

        var appFeature = await scope.ServiceProvider.GetRequiredService<ITenantStorageFactory>().Create(name).GetAppFeaturesAsync();
        appFeature.Should().NotBeNull();
        appFeature!.EventLoggingRetentionPeriod.Should().Be(expectedEventLoggingRetentionPeriod);
        appFeature.EventLoggingIsEnabled.Should().BeTrue();
        appFeature.DeveloperLoggingEndsAt.Should().BeNull();
    }

    [Fact]
    public async Task I_can_get_an_apps_features()
    {
        // Arrange
        const int expectedEventLoggingRetentionPeriod = 30;

        var name = GetApplicationName();
        _ = await _client.CreateApplicationAsync(name);
        var manageAppFeatureRequest = new ManageFeaturesRequest { EventLoggingRetentionPeriod = expectedEventLoggingRetentionPeriod, EventLoggingIsEnabled = true };
        _ = await _client.PostAsJsonAsync($"/admin/apps/{name}/features", manageAppFeatureRequest);

        // Act
        var getAppFeatureResponse = await _client.GetAsync($"/admin/apps/{name}/features");

        //Assert
        getAppFeatureResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var appFeature = await getAppFeatureResponse.Content.ReadFromJsonAsync<AppFeatureResponse>();
        appFeature.Should().NotBeNull();
        appFeature!.EventLoggingRetentionPeriod.Should().Be(expectedEventLoggingRetentionPeriod);
        appFeature.EventLoggingIsEnabled.Should().BeTrue();
        appFeature.DeveloperLoggingEndsAt.Should().BeNull();
        appFeature.IsGenerateSignInTokenEndpointEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task I_can_get_all_api_keys_for_my_application()
    {
        // Arrange
        var applicationName = GetApplicationName();
        using var client = _apiFactory.CreateClient().AddManagementKey();
        _ = await client.CreateApplicationAsync(applicationName);

        // Act
        using var getApiKeysResponse = await client.GetAsync($"/admin/apps/{applicationName}/api-keys");

        // Assert
        getApiKeysResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiKeys = await getApiKeysResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();
        apiKeys.Should().NotBeNullOrEmpty();
        apiKeys!.Where(x => x.Type == ApiKeyTypes.Public).Should().HaveCount(2);
        apiKeys!.Where(x => x.Type == ApiKeyTypes.Secret).Should().HaveCount(2);
    }

    [Fact]
    public async Task I_can_create_a_new_public_key()
    {
        // Arrange
        var applicationName = GetApplicationName();

        _ = await _client.CreateApplicationAsync(applicationName);

        // Act
        using var createApiKeyResponse = await _client.PostAsJsonAsync($"/admin/apps/{applicationName}/public-keys",
            new CreatePublicKeyRequest([PublicKeyScopes.Login, PublicKeyScopes.Register]));

        // Assert
        createApiKeyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiKey = await createApiKeyResponse.Content.ReadFromJsonAsync<CreateApiKeyResponse>();

        using var getApiKeysResponse = await _client.GetAsync($"/admin/apps/{applicationName}/api-keys");
        var apiKeys = await getApiKeysResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();

        apiKeys.Should().NotBeNullOrEmpty();
        apiKeys.Should().Contain(x => x.ApiKey == apiKey!.ApiKey);
    }

    [Fact]
    public async Task I_can_create_a_new_secret_key()
    {
        // Arrange
        var applicationName = GetApplicationName();

        _ = await _client.CreateApplicationAsync(applicationName);

        // Act
        using var createApiKeyResponse = await _client.PostAsJsonAsync($"/admin/apps/{applicationName}/secret-keys",
            new CreateSecretKeyRequest([SecretKeyScopes.TokenRegister, SecretKeyScopes.TokenVerify]));

        // Assert
        createApiKeyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiKey = await createApiKeyResponse.Content.ReadFromJsonAsync<CreateApiKeyResponse>();

        using var getApiKeysResponse = await _client.GetAsync($"/admin/apps/{applicationName}/api-keys");
        var apiKeys = await getApiKeysResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();

        apiKeys.Should().NotBeNullOrEmpty();
        apiKeys.Should().Contain(x => x.ApiKey.GetLast(4) == apiKey!.ApiKey.GetLast(4));
    }

    [Fact]
    public async Task I_can_lock_an_api_key()
    {
        // Arrange
        var applicationName = GetApplicationName();
        _ = await _client.CreateApplicationAsync(applicationName);
        using var getApiKeysResponse = await _client.GetAsync($"/admin/apps/{applicationName}/api-keys");
        var apiKeys = await getApiKeysResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();
        var keyToLock = apiKeys!.First(x => x.Type == ApiKeyTypes.Public);

        // Act
        using var response = await _client.PostAsync($"/admin/apps/{applicationName}/api-keys/{keyToLock.Id}/lock", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var unauthorizedResponse = await _client
            .AddPublicKey(keyToLock.ApiKey)
            .PostAsJsonAsync($"register/begin",
                new FidoRegistrationBeginDTO
                {
                    Origin = PasswordlessApiFactory.OriginUrl,
                    RPID = PasswordlessApiFactory.RpId,
                    Token = "a_bad_token"
                });

        unauthorizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task I_can_unlock_a_locked_api_key()
    {
        // Arrange
        var applicationName = GetApplicationName();
        _ = await _client.CreateApplicationAsync(applicationName);
        using var getApiKeysResponse = await _client.GetAsync($"/admin/apps/{applicationName}/api-keys");
        var apiKeys = await getApiKeysResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();
        var key = apiKeys!.First(x => x.Type == ApiKeyTypes.Public);
        _ = await _client.PostAsync($"/admin/apps/{applicationName}/api-keys/{key.Id}/lock", null);

        // Act
        using var response = await _client.PostAsync($"/admin/apps/{applicationName}/api-keys/{key.Id}/unlock", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var authorizedResponse = await _client
            .AddPublicKey(key.ApiKey)
            .PostAsJsonAsync($"signin/begin",
                new SignInBeginDTO
                {
                    Origin = PasswordlessApiFactory.OriginUrl,
                    RPID = PasswordlessApiFactory.RpId,
                    UserId = "a_user"
                });

        authorizedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task I_can_delete_an_api_key()
    {
        // Arrange
        var applicationName = GetApplicationName();
        _ = await _client.CreateApplicationAsync(applicationName);
        using var getApiKeysResponse = await _client.GetAsync($"/admin/apps/{applicationName}/api-keys");
        var apiKeys = await getApiKeysResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();
        var keyToDelete = apiKeys!.First();

        // Act
        using var responseMessage = await _client.DeleteAsync($"/admin/apps/{applicationName}/api-keys/{keyToDelete.Id}");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var assertKeyIsDeletedResponse = await _client.GetAsync($"/admin/apps/{applicationName}/api-keys");
        var assertKeyIsDeleted = await assertKeyIsDeletedResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ApiKeyResponse>>();

        assertKeyIsDeleted.Should().NotContain(x => x.Id == keyToDelete.Id);
    }

    [Fact]
    public async Task I_can_enable_the_generate_sign_in_token_endpoint()
    {
        // Arrange
        var applicationName = GetApplicationName();
        using var appCreationResponse = await _client.CreateApplicationAsync(applicationName);
        var keysCreation = await appCreationResponse.Content.ReadFromJsonAsync<CreateAppResultDto>();
        _ = _client.AddSecretKey(keysCreation!.ApiSecret1);

        // Act
        using var enableResponse = await _client.PostAsJsonAsync("apps/features",
            new SetFeaturesRequest
            {
                PerformedBy = "a_user",
                EnableManuallyGeneratedAuthenticationTokens = true
            });

        // Assert
        enableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var signInGenerateTokenResponse = await _client.PostAsJsonAsync("signin/generate-token",
            new SigninTokenRequest("some_user")
            {
                Origin = PasswordlessApiFactory.OriginUrl,
                RPID = PasswordlessApiFactory.RpId
            });
        signInGenerateTokenResponse.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task I_can_disable_the_generate_sign_in_token_endpoint()
    {
        // Arrange
        var applicationName = GetApplicationName();
        using var appCreationResponse = await _client.CreateApplicationAsync(applicationName);
        var keysCreation = await appCreationResponse.Content.ReadFromJsonAsync<CreateAppResultDto>();
        _ = _client.AddSecretKey(keysCreation!.ApiSecret1);

        // Act
        using var enableResponse = await _client.PostAsJsonAsync("apps/features",
            new SetFeaturesRequest
            {
                PerformedBy = "a_user",
                EnableManuallyGeneratedAuthenticationTokens = false
            });

        // Assert
        enableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var signInGenerateTokenResponse = await _client.PostAsJsonAsync("signin/generate-token",
            new SigninTokenRequest("some_user")
            {
                Origin = PasswordlessApiFactory.OriginUrl,
                RPID = PasswordlessApiFactory.RpId
            });
        signInGenerateTokenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task I_can_enable_magic_links()
    {
        // Arrange
        var applicationName = GetApplicationName();
        using var appCreationResponse = await _client.CreateApplicationAsync(applicationName);
        var keysCreation = await appCreationResponse.Content.ReadFromJsonAsync<CreateAppResultDto>();
        _ = _client.AddSecretKey(keysCreation!.ApiSecret1);

        // Act
        using var enableResponse = await _client.PostAsJsonAsync("apps/features",
            new SetFeaturesRequest
            {
                PerformedBy = "a_user",
                EnableMagicLinks = true
            });

        // Assert
        enableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var magicLinkRequest = RequestHelpers.GetMagicLinkRequestRules().Generate();

        using var signInGenerateTokenResponse = await _client.PostAsJsonAsync("magic-link/send", magicLinkRequest);
        signInGenerateTokenResponse.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task I_can_disable_magic_links()
    {
        // Arrange
        var applicationName = GetApplicationName();
        using var appCreationResponse = await _client.CreateApplicationAsync(applicationName);
        var keysCreation = await appCreationResponse.Content.ReadFromJsonAsync<CreateAppResultDto>();
        _ = _client.AddSecretKey(keysCreation!.ApiSecret1);

        // Act
        using var enableResponse = await _client.PostAsJsonAsync("apps/features",
            new SetFeaturesRequest
            {
                PerformedBy = "a_user",
                EnableMagicLinks = false
            });

        // Assert
        enableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var magicLinkRequest = RequestHelpers.GetMagicLinkRequestRules().Generate();

        using var signInGenerateTokenResponse = await _client.PostAsJsonAsync("magic-link/send", magicLinkRequest);
        signInGenerateTokenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task I_can_check_whether_an_app_id_is_available()
    {
        // Arrange
        var applicationName = GetApplicationName();

        // Act
        using var response = await _client.GetAsync($"/admin/apps/{applicationName}/available");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetAppIdAvailabilityResponse>();
        result.Should().NotBeNull();
        result!.Available.Should().BeTrue();
    }

    [Fact]
    public async Task I_can_check_whether_an_app_id_is_unavailable()
    {
        // Arrange
        var applicationName = GetApplicationName();
        _ = await _client.CreateApplicationAsync(applicationName);

        // Act
        using var response = await _client.GetAsync($"/admin/apps/{applicationName}/available");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var result = await response.Content.ReadFromJsonAsync<GetAppIdAvailabilityResponse>();
        result.Should().NotBeNull();
        result!.Available.Should().BeFalse();
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}