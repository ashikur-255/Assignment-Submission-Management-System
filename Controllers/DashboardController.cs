using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/dashboard"),Authorize]
public sealed class DashboardController(
    IRepository<User> users,IRepository<ClassRoom> classes,IRepository<Course> courses,IRepository<Subject> subjects,
    IRepository<Assignment> assignments,IRepository<Submission> submissions,IRepository<StudentEnrollment> enrollments,ICurrentUser currentUser):ControllerBase
{
    [HttpGet("admin"),Authorize(Roles=Roles.Admin)]
    public async Task<IActionResult>Admin(CancellationToken ct)
    {
        var u=await users.GetAllAsync(ct);var a=await assignments.GetAllAsync(ct);var s=await submissions.GetAllAsync(ct);
        return Ok(new ApiResponse<object>(true,"Admin dashboard.",new{totalUsers=u.Count,totalTeachers=u.Count(x=>x.Role==Roles.Teacher),totalStudents=u.Count(x=>x.Role==Roles.Student),totalClasses=(await classes.GetAllAsync(ct)).Count,totalCourses=(await courses.GetAllAsync(ct)).Count,totalSubjects=(await subjects.GetAllAsync(ct)).Count,totalEnrollments=(await enrollments.GetAllAsync(ct)).Count,totalAssignments=a.Count,totalSubmissions=s.Count,gradedSubmissions=s.Count(x=>x.Status==SubmissionStatuses.Graded)}));
    }
    [HttpGet("teacher"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Teacher(CancellationToken ct)
    {
        var a=(await assignments.GetAllAsync(ct)).Where(x=>x.TeacherId==currentUser.UserId).ToList();var ids=a.Select(x=>x.Id).ToHashSet();var s=(await submissions.GetAllAsync(ct)).Where(x=>ids.Contains(x.AssignmentId)).ToList();
        return Ok(new ApiResponse<object>(true,"Teacher dashboard.",new{totalAssignments=a.Count,published=a.Count(x=>x.Status==AssignmentStatuses.Published),drafts=a.Count(x=>x.Status==AssignmentStatuses.Draft),totalSubmissions=s.Count,pendingReviews=s.Count(x=>x.Status!=SubmissionStatuses.Graded),graded=s.Count(x=>x.Status==SubmissionStatuses.Graded)}));
    }
    [HttpGet("student"),Authorize(Roles=Roles.Student)]
    public async Task<IActionResult>Student(CancellationToken ct)
    {
        var e=(await enrollments.GetAllAsync(ct)).Where(x=>x.StudentId==currentUser.UserId&&x.IsActive).ToList();var pairs=e.Select(x=>$"{x.ClassId}:{x.CourseId}").ToHashSet();
        var a=(await assignments.GetAllAsync(ct)).Where(x=>x.Status==AssignmentStatuses.Published&&pairs.Contains($"{x.ClassId}:{x.CourseId}")).ToList();var s=(await submissions.GetAllAsync(ct)).Where(x=>x.StudentId==currentUser.UserId).ToList();
        return Ok(new ApiResponse<object>(true,"Student dashboard.",new{totalAssignments=a.Count,pending=a.Count(x=>!s.Any(y=>y.AssignmentId==x.Id)),submitted=s.Count(x=>x.Status!=SubmissionStatuses.Graded),graded=s.Count(x=>x.Status==SubmissionStatuses.Graded)}));
    }
}