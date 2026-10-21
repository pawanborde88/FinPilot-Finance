using FinPilot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinPilot.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
