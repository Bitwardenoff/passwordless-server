using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Passwordless.Service.Models;

namespace Passwordless.Service.Storage.Ef;

public class EfGlobalGlobalStorage : IGlobalStorage
{

    private readonly DbGlobalContext _db;
    private readonly ISystemClock _systemClock;

    public EfGlobalGlobalStorage(DbGlobalContext db, ISystemClock systemClock)
    {
        _db = db;
        _systemClock = systemClock;
    }

    public async Task<ICollection<string>> GetApplicationsPendingDeletionAsync()
    {
        // Will replace IgnoreQueryFilters at a later stage. To possibly use different db contexts.
        var tenants = await _db.AccountInfo
            .Where(x => x.DeleteAt <= _systemClock.UtcNow.UtcDateTime)
            .Select(x => x.Tenant)
            .ToListAsync();
        return tenants;
    }

    public async Task<ApplicationSummary> GetApplicationSummary(string applicationId)
    {
        return await _db.AccountInfo
            .Where(x => x.Tenant == applicationId)
            .Select(x => new ApplicationSummary(x.Tenant, x.DeleteAt)).FirstOrDefaultAsync();
    }
}