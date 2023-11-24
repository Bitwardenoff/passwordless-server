using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Passwordless.AdminConsole.Billing.Configuration;
using Passwordless.AdminConsole.Db;
using Passwordless.AdminConsole.Models;
using Passwordless.AdminConsole.Services;
using Passwordless.AdminConsole.Services.PasswordlessManagement;
using Stripe;

namespace Passwordless.AdminConsole.Billing;

public class NoOpBillingService<TDbContext> : BaseBillingService<TDbContext>, ISharedBillingService where TDbContext : ConsoleDbContext
{
    public NoOpBillingService(
        IDbContextFactory<TDbContext> dbContextFactory,
        IDataService dataService,
        IPasswordlessManagementClient passwordlessClient,
        ILogger<SharedStripeBillingService<TDbContext>> logger,
        IOptions<StripeOptions> stripeOptions,
        IActionContextAccessor actionContextAccessor,
        IUrlHelperFactory urlHelperFactory
    ) : base(dbContextFactory, dataService, passwordlessClient, logger, stripeOptions, actionContextAccessor, urlHelperFactory)
    {
    }

    public Task UpdateUsageAsync()
    {
        // This can be a no-op.
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<PaymentMethodModel>> GetPaymentMethods(string? organizationBillingCustomerId)
    {
        return Task.FromResult<IReadOnlyCollection<PaymentMethodModel>>(Array.Empty<PaymentMethodModel>().ToImmutableList());
    }

    public Task OnSubscriptionCreatedAsync(string customerId, string clientReferenceId, string subscriptionId)
    {
        // only used in webhook
        throw new NotImplementedException();
    }

    public Task UpdateSubscriptionStatusAsync(Invoice? dataObject)
    {
        // Only used in webhook
        throw new NotImplementedException();
    }

    public Task<bool> CancelSubscriptionAsync(string subscriptionId)
    {
        // Deleting org
        // noop
        return Task.FromResult(true);
    }

    public Task<string?> GetCustomerIdAsync(int organizationId)
    {
        // can be noop, only used to open stripe to manage billing
        throw new NotImplementedException();
    }

    public Task OnSubscriptionDeletedAsync(string subscriptionId)
    {
        // only used in webhook
        throw new NotImplementedException();
    }

    public Task OnPostApplicationDeletedAsync(string subscriptionItemId)
    {
        // can be noop
        return Task.CompletedTask;
    }

    public async Task<string?> GetRedirectToUpgradeOrganization(string selectedPlan)
    {
        // Upgrade org
        var organization = await _dataService.GetOrganizationWithDataAsync();

        // Todo: this should be called something better
        await SetFeatures("simple", selectedPlan, organization.Id, "simple", DateTime.UtcNow, "simple","simple");
        
        // I don't link to return these strings
        return "/billing/manage";
    }

    public async Task<string?> ChangePlanAsync(string app, string selectedPlan)
    {
        await this.SetPlanOnApp(app, selectedPlan, "simple", "simple");

        // TODO: returning this string is a bit werid
        return "/billing/manage";
    }

    public Task<(string subscriptionItemId, string priceId)> CreateSubscriptionItem(Organization org, string planSKU)
    {
        return Task.FromResult(new ValueTuple<string, string>("asd", "asd"));
    }
}