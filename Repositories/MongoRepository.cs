using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using AssignmentManagementSystem.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AssignmentManagementSystem.Infrastructure.Repositories;

public sealed class MongoRepository<T> : IRepository<T> where T : MongoEntity
{
    private readonly IMongoCollection<T> _collection;
    public MongoRepository(MongoContext context)
    {
        _collection = typeof(T).Name switch
        {
            nameof(User) => (IMongoCollection<T>)context.Users,
            nameof(RefreshToken) => (IMongoCollection<T>)context.RefreshTokens,
            nameof(ClassRoom) => (IMongoCollection<T>)context.Classes,
            nameof(Course) => (IMongoCollection<T>)context.Courses,
            nameof(Subject) => (IMongoCollection<T>)context.Subjects,
            nameof(StudentEnrollment) => (IMongoCollection<T>)context.StudentEnrollments,
            nameof(TeacherAssignment) => (IMongoCollection<T>)context.TeacherAssignments,
            nameof(Assignment) => (IMongoCollection<T>)context.Assignments,
            nameof(Submission) => (IMongoCollection<T>)context.Submissions,
            nameof(ApplicationSetting) => (IMongoCollection<T>)context.Settings,
            _ => throw new InvalidOperationException($"No collection registered for {typeof(T).Name}")
        };
    }
    public Task<T?> GetByIdAsync(string id,CancellationToken ct=default)=>_collection.Find(x=>x.Id==id).FirstOrDefaultAsync(ct);
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct=default)=>await _collection.Find(FilterDefinition<T>.Empty).SortByDescending(x=>x.CreatedAt).ToListAsync(ct);
    public async Task<T> InsertAsync(
    T entity,
    CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
        }

        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _collection.InsertOneAsync(
            entity,
            cancellationToken: ct
        );

        return entity;
    }
    public async Task UpdateAsync(T entity,CancellationToken ct=default)
    {
        entity.UpdatedAt=DateTime.UtcNow;
        var result=await _collection.ReplaceOneAsync(x=>x.Id==entity.Id,entity,cancellationToken:ct);
        if(result.MatchedCount==0) throw new KeyNotFoundException("Resource not found.");
    }
    public async Task DeleteAsync(
        string id,
        CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(
            x => x.Id == id,
            ct
        );

        if (result.DeletedCount == 0)
        {
            throw new KeyNotFoundException(
                "Resource not found."
            );
        }
    }
}