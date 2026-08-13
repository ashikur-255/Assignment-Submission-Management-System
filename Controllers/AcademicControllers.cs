using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/classes")]
[Authorize]
public sealed class ClassesController(
    IRepository<ClassRoom> repo
) : ControllerBase
{
    // GET: api/classes
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var classes = await repo.GetAllAsync(ct);

        IEnumerable<ClassRoom> query = classes;

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                x.Code.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var total = query.LongCount();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(
            new ApiResponse<PagedResult<ClassRoom>>(
                true,
                "Classes retrieved.",
                new PagedResult<ClassRoom>(
                    items,
                    total,
                    page,
                    pageSize
                )
            )
        );
    }

    // POST: api/classes
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClassRequest request,
        CancellationToken ct = default)
    {
        var name = request.Name?.Trim();
        var code = request.Code?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Class name is required."
                )
            );
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Class code is required."
                )
            );
        }

        var existingClasses = await repo.GetAllAsync(ct);

        if (existingClasses.Any(x =>
            x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(
                new ApiResponse<object>(
                    false,
                    $"Class code '{code}' already exists."
                )
            );
        }

        var entity = new ClassRoom
        {
            Name = name,
            Code = code,
            Description = description ?? string.Empty,
            IsActive = true
        };

        var created = await repo.InsertAsync(entity, ct);

        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponse<ClassRoom>(
                true,
                "Class created successfully.",
                created
            )
        );
    }

    // PUT: api/classes/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateClassRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Class ID is required."
                )
            );
        }

        var entity = await repo.GetByIdAsync(id, ct);

        if (entity is null)
        {
            return NotFound(
                new ApiResponse<object>(
                    false,
                    "Class not found."
                )
            );
        }

        var name = request.Name?.Trim();
        var code = request.Code?.Trim();
        var description = request.Description?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Class name is required."
                )
            );
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Class code is required."
                )
            );
        }

        var existingClasses = await repo.GetAllAsync(ct);

        if (existingClasses.Any(x =>
            x.Id != id &&
            x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(
                new ApiResponse<object>(
                    false,
                    $"Class code '{code}' already exists."
                )
            );
        }

        entity.Name = name;
        entity.Code = code;
        entity.Description = description ?? string.Empty;
        entity.IsActive = request.IsActive;

        await repo.UpdateAsync(entity, ct);

        return Ok(
            new ApiResponse<ClassRoom>(
                true,
                "Class updated successfully.",
                entity
            )
        );
    }

    // DELETE: api/classes/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(
                new ApiResponse<object>(
                    false,
                    "Class ID is required."
                )
            );
        }

        var entity = await repo.GetByIdAsync(id, ct);

        if (entity is null)
        {
            return NotFound(
                new ApiResponse<object>(
                    false,
                    "Class not found."
                )
            );
        }

        await repo.DeleteAsync(id, ct);

        return Ok(
            new ApiResponse<object>(
                true,
                "Class deleted successfully."
            )
        );
    }
}