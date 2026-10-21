using FinPilot.Application.Abstractions;
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
    private readonly IAppDbContext _context;

    public OrganizationsController(IAppDbContext context)
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
    /// Retrieves an organization by its unique Guid identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Organization>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (organization == null)
        {
            return NotFound(new { message = $"Organization with ID '{id}' was not found." });
        }

        return Ok(organization);
    }

    /// <summary>
    /// Retrieves an organization by its unique URL slug.
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Organization>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var organization = await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Slug == normalizedSlug, cancellationToken);

        if (organization == null)
        {
            return NotFound(new { message = $"Organization with slug '{slug}' was not found." });
        }

        return Ok(organization);
    }

    /// <summary>
    /// Creates a new organization in PostgreSQL database.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Organization), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Organization>> Create([FromBody] CreateOrganizationDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { message = "Organization name is required." });
        }

        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            return BadRequest(new { message = "Organization slug is required." });
        }

        try
        {
            var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();
            var exists = await _context.Organizations
                .AnyAsync(o => o.Slug == normalizedSlug, cancellationToken);

            if (exists)
            {
                return Conflict(new { message = $"An organization with slug '{dto.Slug}' already exists." });
            }

            var organization = new Organization(dto.Name, dto.Slug);

            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = organization.Id }, organization);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deactivates an organization.
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations.FindAsync([id], cancellationToken);
        if (organization == null)
        {
            return NotFound(new { message = $"Organization with ID '{id}' was not found." });
        }

        organization.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Activates an organization.
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations.FindAsync([id], cancellationToken);
        if (organization == null)
        {
            return NotFound(new { message = $"Organization with ID '{id}' was not found." });
        }

        organization.Activate();
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Deletes an organization by its unique Guid identifier.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations.FindAsync([id], cancellationToken);

        if (organization == null)
        {
            return NotFound(new { message = $"Organization with ID '{id}' was not found." });
        }

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
