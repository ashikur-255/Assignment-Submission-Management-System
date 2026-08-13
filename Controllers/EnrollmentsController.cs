using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize(Roles = Roles.Admin)]
public sealed class EnrollmentsController : ControllerBase
{
    private readonly IRepository<StudentEnrollment> _enrollments;
    private readonly IRepository<User> _users;
    private readonly IRepository<ClassRoom> _classes;
    private readonly IRepository<Course> _courses;

    public EnrollmentsController(
        IRepository<StudentEnrollment> enrollments,
        IRepository<User> users,
        IRepository<ClassRoom> classes,
        IRepository<Course> courses)
    {
        _enrollments = enrollments;
        _users = users;
        _classes = classes;
        _courses = courses;
    }

    // GET: api/enrollments
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? studentId,
        [FromQuery] string? classId,
        [FromQuery] string? courseId,
        CancellationToken ct = default)
    {
        var query =
            (await _enrollments.GetAllAsync(ct))
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(studentId))
        {
            studentId = studentId.Trim();

            query = query.Where(x =>
                x.StudentId == studentId);
        }

        if (!string.IsNullOrWhiteSpace(classId))
        {
            classId = classId.Trim();

            query = query.Where(x =>
                x.ClassId == classId);
        }

        if (!string.IsNullOrWhiteSpace(courseId))
        {
            courseId = courseId.Trim();

            query = query.Where(x =>
                x.CourseId == courseId);
        }

        var result = query.ToList();

        return Ok(
            new ApiResponse<IReadOnlyList<StudentEnrollment>>(
                true,
                "Enrollments retrieved successfully.",
                result));
    }

    // POST: api/enrollments
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken ct = default)
    {
        var studentId = request.StudentId.Trim();
        var classId = request.ClassId.Trim();
        var courseId = request.CourseId.Trim();

        if (string.IsNullOrWhiteSpace(studentId))
            throw new ArgumentException(
                "Student is required.");

        if (string.IsNullOrWhiteSpace(classId))
            throw new ArgumentException(
                "Class is required.");

        if (string.IsNullOrWhiteSpace(courseId))
            throw new ArgumentException(
                "Course is required.");

        // ----------------------------------------------------
        // 1. Check student
        // ----------------------------------------------------

        var student =
            await _users.GetByIdAsync(
                studentId,
                ct);

        if (student is null)
        {
            throw new KeyNotFoundException(
                "Selected student was not found.");
        }

        if (!student.Role.Equals(
                Roles.Student,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Selected user is not a student.");
        }

        if (!student.IsActive)
        {
            throw new InvalidOperationException(
                "Selected student is inactive.");
        }

        // ----------------------------------------------------
        // 2. Check class
        // ----------------------------------------------------

        var classroom =
            await _classes.GetByIdAsync(
                classId,
                ct);

        if (classroom is null)
        {
            throw new KeyNotFoundException(
                "Selected class was not found.");
        }

        if (!classroom.IsActive)
        {
            throw new InvalidOperationException(
                "Selected class is inactive.");
        }

        // ----------------------------------------------------
        // 3. Check course
        // ----------------------------------------------------

        var course =
            await _courses.GetByIdAsync(
                courseId,
                ct);

        if (course is null)
        {
            throw new KeyNotFoundException(
                "Selected course was not found.");
        }

        if (!course.IsActive)
        {
            throw new InvalidOperationException(
                "Selected course is inactive.");
        }

        // ----------------------------------------------------
        // 4. Make sure course belongs to class
        // ----------------------------------------------------

        if (course.ClassId != classId)
        {
            throw new ArgumentException(
                "Selected course does not belong to the selected class.");
        }

        // ----------------------------------------------------
        // 5. Prevent duplicate enrollment
        // ----------------------------------------------------

        var enrollments =
            await _enrollments.GetAllAsync(ct);

        var alreadyExists =
            enrollments.Any(x =>
                x.StudentId == studentId &&
                x.CourseId == courseId);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "Student is already enrolled in this course.");
        }

        // ----------------------------------------------------
        // 6. Create enrollment
        // ----------------------------------------------------

        var enrollment = new StudentEnrollment
        {
            StudentId = studentId,
            ClassId = classId,
            CourseId = courseId,
            IsActive = true
        };

        var created =
            await _enrollments.InsertAsync(
                enrollment,
                ct);

        return Ok(
            new ApiResponse<StudentEnrollment>(
                true,
                "Student enrolled successfully.",
                created));
    }

    // PUT: api/enrollments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateEnrollmentRequest request,
        CancellationToken ct = default)
    {
        var enrollment =
            await _enrollments.GetByIdAsync(
                id,
                ct);

        if (enrollment is null)
        {
            throw new KeyNotFoundException(
                "Enrollment not found.");
        }

        var classId = request.ClassId.Trim();
        var courseId = request.CourseId.Trim();

        if (string.IsNullOrWhiteSpace(classId))
            throw new ArgumentException(
                "Class is required.");

        if (string.IsNullOrWhiteSpace(courseId))
            throw new ArgumentException(
                "Course is required.");

        var classroom =
            await _classes.GetByIdAsync(
                classId,
                ct);

        if (classroom is null)
        {
            throw new KeyNotFoundException(
                "Selected class was not found.");
        }

        var course =
            await _courses.GetByIdAsync(
                courseId,
                ct);

        if (course is null)
        {
            throw new KeyNotFoundException(
                "Selected course was not found.");
        }

        if (course.ClassId != classId)
        {
            throw new ArgumentException(
                "Selected course does not belong to the selected class.");
        }

        enrollment.ClassId = classId;
        enrollment.CourseId = courseId;
        enrollment.IsActive = request.IsActive;

        await _enrollments.UpdateAsync(
            enrollment,
            ct);

        return Ok(
            new ApiResponse<StudentEnrollment>(
                true,
                "Enrollment updated successfully.",
                enrollment));
    }

    // DELETE: api/enrollments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken ct = default)
    {
        var enrollment =
            await _enrollments.GetByIdAsync(
                id,
                ct);

        if (enrollment is null)
        {
            throw new KeyNotFoundException(
                "Enrollment not found.");
        }

        await _enrollments.DeleteAsync(
            id,
            ct);

        return Ok(
            new ApiResponse<object>(
                true,
                "Enrollment deleted successfully."));
    }
}