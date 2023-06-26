﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Passwordless.AdminConsole.Services;
using Passwordless.Net;

namespace AdminConsole.Pages;

public class CredentialDeleteModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IScopedPasswordlessClient _passwordlessClient;

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string CredentialId { get; set; }

    public Credential Credential { get; set; }

    public CredentialDeleteModel(ILogger<IndexModel> logger, IScopedPasswordlessClient passwordlessClient)
    {
        _logger = logger;
        this._passwordlessClient = passwordlessClient;
    }

    public async Task<IActionResult> OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await _passwordlessClient.DeleteCredential(CredentialId);

        return RedirectToPage("/app/credentials/user", null, new { UserId = UserId });
    }
}
