using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public sealed class CoursesController : ControllerBase
{
    private readonly IRepository<Course> _courses;
    private readonly IRepository<ClassRoom> _classes;

    public CoursesController(
        IRepository<Course> courses,
        IRepository<ClassRoom> classes)
    {
        _courses = courses;
        _classes = classes;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? classId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = (await _courses.GetAllAsync(ct)).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(classId))
        {
            query = query.Where(x => x.ClassId == classId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                $"{x.Name} {x.Code}"
                    .Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var total = query.LongCount();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<Course>(
            items,
            total,
            page,
            pageSize
        );

        return Ok(
            new ApiResponse<PagedResult<Course>>(
                true,
                "Courses retrieved.",
                result
            )
        );
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseRequest request,
        CancellationToken ct = default)
    {
        var name = request.Name.Trim();
        var code = request.Code.Trim();
        var classId = request.ClassId.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Course code is required.");

        if (string.IsNullOrWhiteSpace(classId))
            throw new ArgumentException("Class is required.");

        var selectedClass = await _classes.GetByIdAsync(classId, ct);

        if (selectedClass is null)
            throw new KeyNotFoundException("Selected class was not found.");

        var existing = await _courses.GetAllAsync(ct);

        if (existing.Any(x =>
            x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Course code '{code}' already exists."
            );
        }

        var course = new Course
        {
            Name = name,
            Code = code,
            Description = request.Description?.Trim() ?? string.Empty,
            ClassId = classId,
            IsActive = true
        };

        var created = await _courses.InsertAsync(course, ct);

        return Ok(
            new ApiResponse<Course>(
                true,
                "Course created successfully.",
                created
            )
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(id, ct);

        if (course is null)
            throw new KeyNotFoundException("Course not found.");

        var name = request.Name.Trim();
        var code = request.Code.Trim();
        var classId = request.ClassId.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Course code is required.");

        if (string.IsNullOrWhiteSpace(classId))
            throw new ArgumentException("Class is required.");

        var selectedClass = await _classes.GetByIdAsync(classId, ct);

        if (selectedClass is null)
            throw new KeyNotFoundException("Selected class was not found.");

        var existing = await _courses.GetAllAsync(ct);

        if (existing.Any(x =>
            x.Id != id &&
            x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Course code '{code}' already exists."
            );
        }

        course.Name = name;
        course.Code = code;
        course.Description = request.Description?.Trim() ?? string.Empty;
        course.ClassId = classId;
        course.IsActive = request.IsActive;

        await _courses.UpdateAsync(course, ct);

        return Ok(
            new ApiResponse<Course>(
                true,
                "Course updated successfully.",
                course
            )
        );
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken ct = default)
    {
        var course = await _courses.GetByIdAsync(id, ct);

        if (course is null)
            throw new KeyNotFoundException("Course not found.");

        await _courses.DeleteAsync(id, ct);

        return Ok(
            new ApiResponse<object>(
                true,
                "Course deleted successfully."
            )
        );
    }
}