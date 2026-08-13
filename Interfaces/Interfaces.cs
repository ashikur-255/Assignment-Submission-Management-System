using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;

namespace AssignmentManagementSystem.Core.Interfaces;

public interface IRepository<T> where T : MongoEntity
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> InsertAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

public interface IPasswordService { string Hash(string password); bool Verify(string password, string hash); }
public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
}
public interface ICurrentUser { string UserId { get; } string Role { get; } bool IsAuthenticated { get; } }
public interface IFileStorageService { Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct); bool IsAllowed(IFormFile file); }
public interface IAssignmentService
{
    Task<Assignment> CreateAsync(CreateAssignmentData data, string teacherId, CancellationToken ct);
    Task<Assignment> UpdateAsync(string id, UpdateAssignmentData data, string teacherId, CancellationToken ct);
    Task PublishAsync(string id, string teacherId, CancellationToken ct);
    Task DeleteAsync(string id, string teacherId, CancellationToken ct);
}
public sealed record CreateAssignmentData(string Title,string Description,string ClassId,string CourseId,string SubjectId,DateTime Deadline,decimal MaximumMarks,bool AllowUpdateBeforeDeadline,string? AttachmentUrl);
public sealed record UpdateAssignmentData(string Title,string Description,string ClassId,string CourseId,string SubjectId,DateTime Deadline,decimal MaximumMarks,bool AllowUpdateBeforeDeadline,string? AttachmentUrl);