using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/subjects")]
[Authorize]
public sealed class SubjectsController : ControllerBase
{
    private readonly IRepository<Subject> _subjects;
    private readonly IRepository<Course> _courses;

    public SubjectsController(
        IRepository<Subject> subjects,
        IRepository<Course> courses)
    {
        _subjects = subjects;
        _courses = courses;
    }

    // GET: api/subjects
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? courseId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = (await _subjects.GetAllAsync(ct)).AsEnumerable();

        // Filter by course
        if (!string.IsNullOrWhiteSpace(courseId))
        {
            courseId = courseId.Trim();

            query = query.Where(x =>
                x.CourseId == courseId);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                $"{x.Name} {x.Code}"
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase));
        }

        var total = query.LongCount();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<Subject>(
            items,
            total,
            page,
            pageSize);

        return Ok(
            new ApiResponse<PagedResult<Subject>>(
                true,
                "Subjects retrieved successfully.",
                result));
    }

    // POST: api/subjects
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectRequest request,
        CancellationToken ct = default)
    {
        var name = request.Name.Trim();
        var code = request.Code.Trim();
        var courseId = request.CourseId.Trim();

        // Validate name
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Subject name is required.");
        }

        // Validate code
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Subject code is required.");
        }

        // Validate course
        if (string.IsNullOrWhiteSpace(courseId))
        {
            throw new ArgumentException(
                "Course is required.");
        }

        // Check course exists
        var course = await _courses.GetByIdAsync(
            courseId,
            ct);

        if (course is null)
        {
            throw new KeyNotFoundException(
                "Selected course was not found.");
        }

        // Prevent duplicate subject code
        var existingSubjects =
            await _subjects.GetAllAsync(ct);

        if (existingSubjects.Any(x =>
            x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Subject code '{code}' already exists.");
        }

        // Create subject
        var subject = new Subject
        {
            Name = name,
            Code = code,
            Description =
                request.Description?.Trim()
                ?? string.Empty,
            CourseId = courseId,
            IsActive = true
        };

        var created =
            await _subjects.InsertAsync(
                subject,
                ct);

        return Ok(
            new ApiResponse<Subject>(
                true,
                "Subject created successfully.",
                created));
    }

    // PUT: api/subjects/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateSubjectRequest request,
        CancellationToken ct = default)
    {
        var subject =
            await _subjects.GetByIdAsync(id, ct);

        if (subject is null)
        {
            throw new KeyNotFoundException(
                "Subject not found.");
        }

        var name = request.Name.Trim();
        var code = request.Code.Trim();
        var courseId = request.CourseId.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Subject name is required.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Subject code is required.");
        }

        if (string.IsNullOrWhiteSpace(courseId))
        {
            throw new ArgumentException(
                "Course is required.");
        }

        // Check course exists
        var course =
            await _courses.GetByIdAsync(
                courseId,
                ct);

        if (course is null)
        {
            throw new KeyNotFoundException(
                "Selected course was not found.");
        }

        // Prevent duplicate code
        var existingSubjects =
            await _subjects.GetAllAsync(ct);

        if (existingSubjects.Any(x =>
            x.Id != id &&
            x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Subject code '{code}' already exists.");
        }

        subject.Name = name;
        subject.Code = code;
        subject.Description =
            request.Description?.Trim()
            ?? string.Empty;
        subject.CourseId = courseId;
        subject.IsActive = request.IsActive;

        await _subjects.UpdateAsync(
            subject,
            ct);

        return Ok(
            new ApiResponse<Subject>(
                true,
                "Subject updated successfully.",
                subject));
    }

    // DELETE: api/subjects/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken ct = default)
    {
        var subject =
            await _subjects.GetByIdAsync(id, ct);

        if (subject is null)
        {
            throw new KeyNotFoundException(
                "Subject not found.");
        }

        await _subjects.DeleteAsync(
            id,
            ct);

        return Ok(
            new ApiResponse<object>(
                true,
                "Subject deleted successfully."));
    }
}