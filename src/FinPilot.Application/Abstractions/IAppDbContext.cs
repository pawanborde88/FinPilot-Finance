using FinPilot.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace FinPilot.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Organization> Organizations { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
