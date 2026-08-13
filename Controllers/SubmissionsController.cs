using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/submissions"),Authorize]
public sealed class SubmissionsController(IRepository<Submission> submissions,IRepository<Assignment> assignments,IRepository<StudentEnrollment> enrollments,ICurrentUser currentUser):ControllerBase
{
    [HttpGet("my"),Authorize(Roles=Roles.Student)]
    public async Task<IActionResult>My([FromQuery]string? status,CancellationToken ct=default){var q=(await submissions.GetAllAsync(ct)).Where(x=>x.StudentId==currentUser.UserId);if(status!=null)q=q.Where(x=>x.Status.Equals(status,StringComparison.OrdinalIgnoreCase));return Ok(new ApiResponse<IReadOnlyList<Submission>>(true,"Submissions retrieved.",q.ToList()));}

    [HttpGet("assignment/{assignmentId}"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>ByAssignment(string assignmentId,[FromQuery]string? status,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        var a=await assignments.GetByIdAsync(assignmentId,ct)??throw new KeyNotFoundException("Assignment not found.");if(a.TeacherId!=currentUser.UserId)throw new UnauthorizedAccessException("You cannot review this assignment.");
        page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=(await submissions.GetAllAsync(ct)).Where(x=>x.AssignmentId==assignmentId);if(status!=null)q=q.Where(x=>x.Status.Equals(status,StringComparison.OrdinalIgnoreCase));
        var total=q.LongCount();return Ok(new ApiResponse<PagedResult<Submission>>(true,"Submissions retrieved.",new(q.Skip((page-1)*pageSize).Take(pageSize).ToList(),total,page,pageSize)));
    }

    [HttpPost,Authorize(Roles=Roles.Student)]
    public async Task<IActionResult>Create(CreateSubmissionRequest r,CancellationToken ct)
    {
        var a=await assignments.GetByIdAsync(r.AssignmentId,ct)??throw new KeyNotFoundException("Assignment not found.");
        var enrolled=(await enrollments.GetAllAsync(ct)).Any(x=>x.StudentId==currentUser.UserId&&x.IsActive&&x.ClassId==a.ClassId&&x.CourseId==a.CourseId);
        if(!enrolled)throw new UnauthorizedAccessException("You are not enrolled in this assignment's class/course.");
        if(a.Status!=AssignmentStatuses.Published)throw new InvalidOperationException("Assignment is not available for submission.");
        if(DateTime.UtcNow>a.Deadline)throw new InvalidOperationException("The submission deadline has passed.");
        var existing=(await submissions.GetAllAsync(ct)).FirstOrDefault(x=>x.AssignmentId==a.Id&&x.StudentId==currentUser.UserId);
        if(existing is not null)
        {
            if(!a.AllowUpdateBeforeDeadline||existing.Status==SubmissionStatuses.Graded)throw new InvalidOperationException("Updating this submission is not allowed.");
            existing.Answer=r.Answer;existing.AttachmentUrl=r.AttachmentUrl;existing.SubmittedAt=DateTime.UtcNow;await submissions.UpdateAsync(existing,ct);
            return Ok(new ApiResponse<Submission>(true,"Submission updated.",existing));
        }
        var s=await submissions.InsertAsync(new Submission{AssignmentId=a.Id,StudentId=currentUser.UserId,Answer=r.Answer,AttachmentUrl=r.AttachmentUrl,SubmittedAt=DateTime.UtcNow,Status=SubmissionStatuses.Submitted},ct);
        return Ok(new ApiResponse<Submission>(true,"Assignment submitted.",s));
    }

    [HttpPut("{id}"),Authorize(Roles=Roles.Student)]
    public async Task<IActionResult>Update(string id,UpdateSubmissionRequest r,CancellationToken ct)
    {
        var s=await submissions.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Submission not found.");if(s.StudentId!=currentUser.UserId)throw new UnauthorizedAccessException("You can only update your own submission.");
        var a=await assignments.GetByIdAsync(s.AssignmentId,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(DateTime.UtcNow>a.Deadline||!a.AllowUpdateBeforeDeadline||s.Status==SubmissionStatuses.Graded)throw new InvalidOperationException("This submission can no longer be edited.");
        s.Answer=r.Answer;s.AttachmentUrl=r.AttachmentUrl;await submissions.UpdateAsync(s,ct);return Ok(new ApiResponse<Submission>(true,"Submission updated.",s));
    }

    [HttpPatch("{id}/grade"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Grade(string id,GradeSubmissionRequest r,CancellationToken ct)
    {
        var s=await submissions.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Submission not found.");var a=await assignments.GetByIdAsync(s.AssignmentId,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(a.TeacherId!=currentUser.UserId)throw new UnauthorizedAccessException("You cannot grade this submission.");if(r.Marks<0||r.Marks>a.MaximumMarks)throw new ArgumentException($"Marks must be between 0 and {a.MaximumMarks}.");
        s.Marks=r.Marks;s.Feedback=r.Feedback;s.GradedAt=DateTime.UtcNow;s.GradedBy=currentUser.UserId;s.Status=SubmissionStatuses.Graded;await submissions.UpdateAsync(s,ct);
        return Ok(new ApiResponse<Submission>(true,"Submission graded.",s));
    }

    [HttpPatch("{id}/status"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Status(string id,ChangeSubmissionStatusRequest r,CancellationToken ct)
    {
        if(!new[]{SubmissionStatuses.Submitted,SubmissionStatuses.Late,SubmissionStatuses.Graded,SubmissionStatuses.Returned}.Contains(r.Status))throw new ArgumentException("Invalid submission status.");
        var s=await submissions.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Submission not found.");var a=await assignments.GetByIdAsync(s.AssignmentId,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(a.TeacherId!=currentUser.UserId)throw new UnauthorizedAccessException("You cannot change this submission.");
        s.Status=r.Status;await submissions.UpdateAsync(s,ct);return Ok(new ApiResponse<Submission>(true,"Submission status updated.",s));
    }
}