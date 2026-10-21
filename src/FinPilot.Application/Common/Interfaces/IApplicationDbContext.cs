using FinPilot.Domain.Entities;
using FinPilot.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace FinPilot.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Transaction> Transactions { get; }
    DbSet<Organization> Organizations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
