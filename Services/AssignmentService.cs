using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;

namespace AssignmentManagementSystem.Infrastructure.Services;

public sealed class AssignmentService(IRepository<Assignment> repository,IRepository<TeacherAssignment> teacherAssignments):IAssignmentService
{
    public async Task<Assignment>CreateAsync(CreateAssignmentData d,string teacherId,CancellationToken ct)
    {
        Validate(d.Deadline,d.MaximumMarks);
        var links=await teacherAssignments.GetAllAsync(ct);
        if(!links.Any(x=>x.IsActive&&x.TeacherId==teacherId&&x.ClassId==d.ClassId&&x.CourseId==d.CourseId&&x.SubjectId==d.SubjectId))
            throw new InvalidOperationException("Teacher is not assigned to the selected class, course and subject.");
        return await repository.InsertAsync(new Assignment
        {
            Title=d.Title.Trim(),Description=d.Description.Trim(),TeacherId=teacherId,ClassId=d.ClassId,CourseId=d.CourseId,SubjectId=d.SubjectId,
            Deadline=d.Deadline.ToUniversalTime(),MaximumMarks=d.MaximumMarks,AllowUpdateBeforeDeadline=d.AllowUpdateBeforeDeadline,
            AttachmentUrl=d.AttachmentUrl,Status=AssignmentStatuses.Draft
        },ct);
    }
    public async Task<Assignment>UpdateAsync(string id,UpdateAssignmentData d,string teacherId,CancellationToken ct)
    {
        Validate(d.Deadline,d.MaximumMarks);
        var x=await repository.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(x.TeacherId!=teacherId)throw new UnauthorizedAccessException("You can only update your own assignments.");
        if(x.Status==AssignmentStatuses.Closed)throw new InvalidOperationException("Closed assignments cannot be edited.");
        x.Title=d.Title.Trim();x.Description=d.Description.Trim();x.ClassId=d.ClassId;x.CourseId=d.CourseId;x.SubjectId=d.SubjectId;
        x.Deadline=d.Deadline.ToUniversalTime();x.MaximumMarks=d.MaximumMarks;x.AllowUpdateBeforeDeadline=d.AllowUpdateBeforeDeadline;x.AttachmentUrl=d.AttachmentUrl;
        await repository.UpdateAsync(x,ct);return x;
    }
    public async Task PublishAsync(string id,string teacherId,CancellationToken ct)
    {
        var x=await repository.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(x.TeacherId!=teacherId)throw new UnauthorizedAccessException("You can only publish your own assignments.");
        if(x.Deadline<=DateTime.UtcNow)throw new InvalidOperationException("Deadline must be in the future.");
        x.Status=AssignmentStatuses.Published;await repository.UpdateAsync(x,ct);
    }
    public async Task DeleteAsync(string id,string teacherId,CancellationToken ct)
    {
        var x=await repository.GetByIdAsync(id,ct)??throw new KeyNotFoundException("Assignment not found.");
        if(x.TeacherId!=teacherId)throw new UnauthorizedAccessException("You can only delete your own assignments.");
        if(x.Status==AssignmentStatuses.Published)throw new InvalidOperationException("Published assignments cannot be deleted.");
        await repository.DeleteAsync(id,ct);
    }
    private static void Validate(DateTime deadline,decimal marks)
    {
        if(deadline.ToUniversalTime()<=DateTime.UtcNow)throw new ArgumentException("Deadline must be in the future.");
        if(marks<=0)throw new ArgumentException("Maximum marks must be greater than zero.");
    }
}