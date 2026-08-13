using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/teacher-assignments")]
[Authorize(Roles = Roles.Admin)]
public sealed class TeacherAssignmentsController : ControllerBase
{
    private readonly IRepository<TeacherAssignment> _assignments;
    private readonly IRepository<User> _users;
    private readonly IRepository<ClassRoom> _classes;
    private readonly IRepository<Course> _courses;
    private readonly IRepository<Subject> _subjects;

    public TeacherAssignmentsController(
        IRepository<TeacherAssignment> assignments,
        IRepository<User> users,
        IRepository<ClassRoom> classes,
        IRepository<Course> courses,
        IRepository<Subject> subjects)
    {
        _assignments = assignments;
        _users = users;
        _classes = classes;
        _courses = courses;
        _subjects = subjects;
    }

    // GET: api/teacher-assignments
    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken ct = default)
    {
        var result =
            await _assignments.GetAllAsync(ct);

        return Ok(
            new ApiResponse<IReadOnlyList<TeacherAssignment>>(
                true,
                "Teacher assignments retrieved successfully.",
                result));
    }

    // POST: api/teacher-assignments
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] AssignTeacherRequest request,
        CancellationToken ct = default)
    {
        var teacherId = request.TeacherId.Trim();
        var classId = request.ClassId.Trim();
        var courseId = request.CourseId.Trim();
        var subjectId = request.SubjectId.Trim();

        // ----------------------------------------------------
        // Validate IDs
        // ----------------------------------------------------

        if (string.IsNullOrWhiteSpace(teacherId))
            throw new ArgumentException(
                "Teacher is required.");

        if (string.IsNullOrWhiteSpace(classId))
            throw new ArgumentException(
                "Class is required.");

        if (string.IsNullOrWhiteSpace(courseId))
            throw new ArgumentException(
                "Course is required.");

        if (string.IsNullOrWhiteSpace(subjectId))
            throw new ArgumentException(
                "Subject is required.");

        // ----------------------------------------------------
        // 1. Check teacher
        // ----------------------------------------------------

        var teacher =
            await _users.GetByIdAsync(
                teacherId,
                ct);

        if (teacher is null)
        {
            throw new KeyNotFoundException(
                "Selected teacher was not found.");
        }

        if (!teacher.Role.Equals(
                Roles.Teacher,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Selected user is not a teacher.");
        }

        if (!teacher.IsActive)
        {
            throw new InvalidOperationException(
                "Selected teacher is inactive.");
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
        // 4. Course must belong to class
        // ----------------------------------------------------

        if (course.ClassId != classId)
        {
            throw new ArgumentException(
                "Selected course does not belong to the selected class.");
        }

        // ----------------------------------------------------
        // 5. Check subject
        // ----------------------------------------------------

        var subject =
            await _subjects.GetByIdAsync(
                subjectId,
                ct);

        if (subject is null)
        {
            throw new KeyNotFoundException(
                "Selected subject was not found.");
        }

        if (!subject.IsActive)
        {
            throw new InvalidOperationException(
                "Selected subject is inactive.");
        }

        // ----------------------------------------------------
        // 6. Subject must belong to course
        // ----------------------------------------------------

        if (subject.CourseId != courseId)
        {
            throw new ArgumentException(
                "Selected subject does not belong to the selected course.");
        }

        // ----------------------------------------------------
        // 7. Prevent duplicate assignment
        // ----------------------------------------------------

        var existingAssignments =
            await _assignments.GetAllAsync(ct);

        var duplicate =
            existingAssignments.Any(x =>
                x.TeacherId == teacherId &&
                x.ClassId == classId &&
                x.CourseId == courseId &&
                x.SubjectId == subjectId);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "This teacher is already assigned to this class, course and subject.");
        }

        // ----------------------------------------------------
        // 8. Create assignment
        // ----------------------------------------------------

        var assignment = new TeacherAssignment
        {
            TeacherId = teacherId,
            ClassId = classId,
            CourseId = courseId,
            SubjectId = subjectId,
            IsActive = true
        };

        var created =
            await _assignments.InsertAsync(
                assignment,
                ct);

        return Ok(
            new ApiResponse<TeacherAssignment>(
                true,
                "Teacher assigned successfully.",
                created));
    }

    // DELETE: api/teacher-assignments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken ct = default)
    {
        var assignment =
            await _assignments.GetByIdAsync(
                id,
                ct);

        if (assignment is null)
        {
            throw new KeyNotFoundException(
                "Teacher assignment not found.");
        }

        await _assignments.DeleteAsync(
            id,
            ct);

        return Ok(
            new ApiResponse<object>(
                true,
                "Teacher assignment removed successfully."));
    }
}