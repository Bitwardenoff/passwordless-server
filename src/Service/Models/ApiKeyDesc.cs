namespace Passwordless.Service.Models;

public class ApiKeyDesc : PerTenant
{
    public required string Id { get; set; }

    // Should be removed
    [Obsolete]
    public string? AccountName { get; set; }

    public required string ApiKey { get; set; }
    public required string[] Scopes { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LastLockedAt { get; set; }
    public DateTime? LastUnlockedAt { get; set; }

    public string MaskedApiKey => ApiKey.Contains("public")
        ? $"{Tenant}:public:{Id.PadLeft(32, '*')}"
        : $"{Tenant}:secret:{Id.PadLeft(32, '*')}";
}