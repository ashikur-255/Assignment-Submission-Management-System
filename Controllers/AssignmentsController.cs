using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/assignments"),Authorize]
public sealed class AssignmentsController(
    IRepository<Assignment> assignments,IRepository<StudentEnrollment> enrollments,ICurrentUser currentUser,IAssignmentService service):ControllerBase
{
    [HttpGet,Authorize(Roles=Roles.Admin)]
    public async Task<IActionResult>GetAll([FromQuery]string? search,[FromQuery]string? status,[FromQuery]string? classId,[FromQuery]string? courseId,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=(await assignments.GetAllAsync(ct)).AsEnumerable();
        if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>$"{x.Title} {x.Description}".Contains(search,StringComparison.OrdinalIgnoreCase));
        if(status!=null)q=q.Where(x=>x.Status.Equals(status,StringComparison.OrdinalIgnoreCase));
        if(classId!=null)q=q.Where(x=>x.ClassId==classId);if(courseId!=null)q=q.Where(x=>x.CourseId==courseId);
        var total=q.LongCount();return Ok(new ApiResponse<PagedResult<Assignment>>(true,"Assignments retrieved.",new(q.Skip((page-1)*pageSize).Take(pageSize).ToList(),total,page,pageSize)));
    }

    [HttpGet("my"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>My([FromQuery]string? search,[FromQuery]string? status,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=(await assignments.GetAllAsync(ct)).Where(x=>x.TeacherId==currentUser.UserId);
        if(search!=null)q=q.Where(x=>$"{x.Title} {x.Description}".Contains(search,StringComparison.OrdinalIgnoreCase));if(status!=null)q=q.Where(x=>x.Status.Equals(status,StringComparison.OrdinalIgnoreCase));
        var total=q.LongCount();return Ok(new ApiResponse<PagedResult<Assignment>>(true,"Assignments retrieved.",new(q.Skip((page-1)*pageSize).Take(pageSize).ToList(),total,page,pageSize)));
    }

    [HttpGet("student"),Authorize(Roles=Roles.Student)]
    public async Task<IActionResult>Student([FromQuery]string? search,[FromQuery]string? status,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);
        var enrolled=(await enrollments.GetAllAsync(ct)).Where(x=>x.StudentId==currentUser.UserId&&x.IsActive).ToList();
        var pairs=enrolled.Select(x=>$"{x.ClassId}:{x.CourseId}").ToHashSet();
        var q=(await assignments.GetAllAsync(ct)).Where(x=>x.Status==AssignmentStatuses.Published&&pairs.Contains($"{x.ClassId}:{x.CourseId}"));
        if(search!=null)q=q.Where(x=>$"{x.Title} {x.Description}".Contains(search,StringComparison.OrdinalIgnoreCase));if(status!=null)q=q.Where(x=>x.Status.Equals(status,StringComparison.OrdinalIgnoreCase));
        var total=q.LongCount();return Ok(new ApiResponse<PagedResult<Assignment>>(true,"Student assignments retrieved.",new(q.Skip((page-1)*pageSize).Take(pageSize).ToList(),total,page,pageSize)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>Get(string id,CancellationToken ct)
    {
        var x=await assignments.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(currentUser.Role==Roles.Teacher&&x.TeacherId!=currentUser.UserId)throw new UnauthorizedAccessException("You cannot view this assignment.");
        if(currentUser.Role==Roles.Student)
        {
            var ok=(await enrollments.GetAllAsync(ct)).Any(e=>e.StudentId==currentUser.UserId&&e.IsActive&&e.ClassId==x.ClassId&&e.CourseId==x.CourseId);
            if(x.Status!=AssignmentStatuses.Published||!ok)throw new UnauthorizedAccessException("You cannot view this assignment.");
        }
        return Ok(new ApiResponse<Assignment>(true,"Assignment retrieved.",x));
    }

    [HttpPost,Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Create(CreateAssignmentRequest r,CancellationToken ct)=>Ok(new ApiResponse<Assignment>(true,"Assignment created as draft.",await service.CreateAsync(new(r.Title,r.Description,r.ClassId,r.CourseId,r.SubjectId,r.Deadline,r.MaximumMarks,r.AllowUpdateBeforeDeadline,r.AttachmentUrl),currentUser.UserId,ct)));

    [HttpPut("{id}"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Update(string id,UpdateAssignmentRequest r,CancellationToken ct)=>Ok(new ApiResponse<Assignment>(true,"Assignment updated.",await service.UpdateAsync(id,new(r.Title,r.Description,r.ClassId,r.CourseId,r.SubjectId,r.Deadline,r.MaximumMarks,r.AllowUpdateBeforeDeadline,r.AttachmentUrl),currentUser.UserId,ct)));

    [HttpPatch("{id}/publish"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Publish(string id,CancellationToken ct){await service.PublishAsync(id,currentUser.UserId,ct);return Ok(new ApiResponse<object>(true,"Assignment published."));}

    [HttpDelete("{id}"),Authorize(Roles=Roles.Teacher)]
    public async Task<IActionResult>Delete(string id,CancellationToken ct){await service.DeleteAsync(id,currentUser.UserId,ct);return Ok(new ApiResponse<object>(true,"Assignment deleted."));}
}