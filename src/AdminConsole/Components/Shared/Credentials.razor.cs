using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Passwordless.AdminConsole.Helpers;
using Passwordless.Common.Extensions;
using Passwordless.Common.Validation;

namespace Passwordless.AdminConsole.Components.Shared;

public partial class Credentials : ComponentBase
{
    public const string ManageCredentialFormName = "manage-credential-form";

    public required IReadOnlyCollection<Credential>? Items { get; set; }

    public IReadOnlyCollection<CredentialModel> GetItems() =>
        Items?.Select(x => new CredentialModel(
            x.Descriptor.Id,
            x.PublicKey,
            x.SignatureCounter,
            x.AttestationFmt,
            x.CreatedAt,
            x.AaGuid,
            x.LastUsedAt,
            x.RpId,
            x.Origin,
            x.Device,
            x.Nickname,
            x.BackupState,
            x.IsBackupEligible,
            x.IsDiscoverable,
            x.AuthenticatorDisplayName ?? AuthenticatorDataProvider.GetName(x.AaGuid))
        ).ToArray() ?? [];

    /// <summary>
    /// Determines whether the details of the credentials should be hidden.
    /// </summary>
    [Parameter]
    public bool HideDetails { get; set; }

    [Parameter]
    public required IPasswordlessClient PasswordlessClient { get; set; }

    [Parameter]
    public required string UserId { get; set; }

    [SupplyParameterFromForm(FormName = ManageCredentialFormName)]
    public ManageCredentialFormModel ManageCredentialForm { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Items = await PasswordlessClient.ListCredentialsAsync(UserId);
    }

    public async Task OnManageCredentialSubmittedAsync()
    {
        var validationContext = new ValidationContext(ManageCredentialForm);
        var validationResult = Validator.TryValidateObject(ManageCredentialForm, validationContext, null, true);
        if (!validationResult)
        {
            throw new ArgumentException("The request is not valid.");
        }
        await PasswordlessClient.DeleteCredentialAsync(ManageCredentialForm.CredentialId);
        NavigationManager.NavigateTo(NavigationManager.Uri);

    }

    /// <summary>
    /// Credential view model
    /// </summary>
    public record CredentialModel
    {
        public string DescriptorId { get; }

        public byte[] PublicKey { get; }

        public uint SignatureCounter { get; }

        public string AttestationFmt { get; }

        public DateTime CreatedAt { get; }

        public Guid AaGuid { get; }

        public DateTime LastUsedAt { get; }

        public string RPID { get; }

        public string Origin { get; }

        public string Device { get; }

        public string Nickname { get; }

        public bool? BackupState { get; }

        public bool? IsBackupEligible { get; }

        public bool? IsDiscoverable { get; }

        public string? AuthenticatorName { get; set; }

        public bool IsNew()
        {
            return CreatedAt > DateTime.UtcNow.AddMinutes(-1);
        }

        /// <summary>
        /// The title of the credential card.
        /// </summary>
        public string Title => AuthenticatorName?.NullIfEmpty() ?? Device.NullIfEmpty() ?? "Passkey";

        private string? _subtitle;

        /// <summary>
        /// The subtitle of the credential card.
        /// </summary>
        public string SubTitle => _subtitle ??= AuthenticatorName switch
        {
            null => Nickname,
            _ => !string.IsNullOrEmpty(Nickname) ? $"{Nickname} on {Device}" : Device
        };

        public CredentialModel(
            byte[] descriptorId,
            byte[] publicKey,
            uint signatureCounter,
            string attestationFmt,
            DateTime createdAt,
            Guid aaGuid,
            DateTime lastUsedAt,
            string rpid,
            string origin,
            string device,
            string nickname,
            bool? backupState,
            bool? isBackupEligible,
            bool? isDiscoverable,
            string? authenticatorName)
        {
            DescriptorId = descriptorId.ToBase64Url();
            PublicKey = publicKey;
            SignatureCounter = signatureCounter;
            AttestationFmt = attestationFmt;
            CreatedAt = createdAt;
            AaGuid = aaGuid;
            LastUsedAt = lastUsedAt;
            RPID = rpid;
            Origin = origin;
            Device = device;
            Nickname = nickname;
            BackupState = backupState;
            IsBackupEligible = isBackupEligible;
            IsDiscoverable = isDiscoverable;
            AuthenticatorName = authenticatorName;
        }
    }

    public sealed class ManageCredentialFormModel
    {
        [Base64Url]
        public string CredentialId { get; set; }
    }
}