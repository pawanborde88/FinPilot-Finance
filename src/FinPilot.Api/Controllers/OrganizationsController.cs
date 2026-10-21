using FinPilot.Application.Common.Interfaces;
using FinPilot.Application.Organizations.Dtos;
using FinPilot.Domain.Organizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinPilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrganizationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public OrganizationsController(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all organizations from PostgreSQL database.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Organization>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Organization>>> GetAll(CancellationToken cancellationToken)
    {
        var organizations = await _context.Organizations
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(organizations);
    }

    /// <summary>
    /// Retrieves an organization by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Organization>> GetById(int id, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (organization == null)
        {
            return NotFound(new { message = $"Organization with ID {id} was not found." });
        }

        return Ok(organization);
    }

    /// <summary>
    /// Creates a new organization in PostgreSQL database.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Organization>> Create([FromBody] CreateOrganizationDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { message = "Organization Name is required." });
        }

        var organization = new Organization
        {
            Name = dto.Name.Trim(),
            Code = dto.Code?.Trim(),
            Description = dto.Description?.Trim(),
            Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "USD" : dto.Currency.Trim().ToUpperInvariant(),
            TimeZone = dto.TimeZone?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = organization.Id }, organization);
    }

    /// <summary>
    /// Deletes an organization by ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations.FindAsync([id], cancellationToken);

        if (organization == null)
        {
            return NotFound(new { message = $"Organization with ID {id} was not found." });
        }

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
