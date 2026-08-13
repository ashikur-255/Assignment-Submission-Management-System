using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using AssignmentManagementSystem.Infrastructure.Services;
using Xunit;

namespace AssignmentManagementSystem.Tests;

public sealed class AssignmentBusinessRulesTests
{
    [Fact]
    public async Task Teacher_Cannot_Update_Another_Teachers_Assignment()
    {
        var repo=new FakeRepository<Assignment>();var teacherRepo=new FakeRepository<TeacherAssignment>();
        await repo.InsertAsync(new Assignment{Id="a1",TeacherId="teacher-1",Status=AssignmentStatuses.Draft,Deadline=DateTime.UtcNow.AddDays(2),MaximumMarks=100});
        var service=new AssignmentService(repo,teacherRepo);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(()=>service.UpdateAsync("a1",new("T","D","c","co","s",DateTime.UtcNow.AddDays(3),100,true,null),"teacher-2",CancellationToken.None));
    }

    [Fact]
    public async Task Teacher_Cannot_Create_Without_Assignment_Link()
    {
        var repo=new FakeRepository<Assignment>();var teacherRepo=new FakeRepository<TeacherAssignment>();var service=new AssignmentService(repo,teacherRepo);
        await Assert.ThrowsAsync<InvalidOperationException>(()=>service.CreateAsync(new("T","D","c","co","s",DateTime.UtcNow.AddDays(2),100,true,null),"teacher-1",CancellationToken.None));
    }

    [Fact]
    public async Task Invalid_Deadline_Is_Rejected()
    {
        var repo=new FakeRepository<Assignment>();var teacherRepo=new FakeRepository<TeacherAssignment>();
        await teacherRepo.InsertAsync(new TeacherAssignment{TeacherId="t",ClassId="c",CourseId="co",SubjectId="s"});
        var service=new AssignmentService(repo,teacherRepo);
        await Assert.ThrowsAsync<ArgumentException>(()=>service.CreateAsync(new("T","D","c","co","s",DateTime.UtcNow.AddMinutes(-1),100,true,null),"t",CancellationToken.None));
    }

    private sealed class FakeRepository<T>:IRepository<T> where T:MongoEntity
    {
        private readonly List<T> items=[];
        public Task<T?>GetByIdAsync(string id,CancellationToken ct=default)=>Task.FromResult(items.FirstOrDefault(x=>x.Id==id));
        public Task<IReadOnlyList<T>>GetAllAsync(CancellationToken ct=default)=>Task.FromResult<IReadOnlyList<T>>(items);
        public Task<T>InsertAsync(T e,CancellationToken ct=default){if(string.IsNullOrWhiteSpace(e.Id))e.Id=Guid.NewGuid().ToString();items.Add(e);return Task.FromResult(e);}
        public Task UpdateAsync(T e,CancellationToken ct=default)=>Task.CompletedTask;
        public Task DeleteAsync(string id,CancellationToken ct=default)=>Task.CompletedTask;
    }
}