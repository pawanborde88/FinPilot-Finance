using FinPilot.Application.Common.Interfaces;
using FinPilot.Application.Transactions.Dtos;
using FinPilot.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public TransactionsController(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all transactions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAll(CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .AsNoTracking()
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves a transaction by its identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Transaction>> GetById(int id, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (transaction == null)
        {
            return NotFound(new { message = $"Transaction with ID {id} was not found." });
        }

        return Ok(transaction);
    }

    /// <summary>
    /// Creates a new financial transaction.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Transaction), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Transaction>> Create([FromBody] CreateTransactionDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest(new { message = "Title is required." });
        }

        if (dto.Amount <= 0)
        {
            return BadRequest(new { message = "Amount must be greater than zero." });
        }

        var transaction = new Transaction
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Amount = dto.Amount,
            Type = dto.Type,
            Category = dto.Category?.Trim() ?? "General",
            TransactionDate = dto.TransactionDate ?? DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    /// <summary>
    /// Deletes a transaction by its identifier.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.FindAsync([id], cancellationToken);

        if (transaction == null)
        {
            return NotFound(new { message = $"Transaction with ID {id} was not found." });
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
