using AssignmentManagementSystem.Core.Models;
using AssignmentManagementSystem.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssignmentManagementSystem.Infrastructure.Data;

public sealed class MongoContext
{
    private readonly IMongoDatabase _database;
    public MongoContext(IOptions<MongoSettings> options)
    {
        _database = new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName);
    }
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("refreshTokens");
    public IMongoCollection<ClassRoom> Classes => _database.GetCollection<ClassRoom>("classes");
    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("courses");
    public IMongoCollection<Subject> Subjects => _database.GetCollection<Subject>("subjects");
    public IMongoCollection<StudentEnrollment> StudentEnrollments => _database.GetCollection<StudentEnrollment>("studentEnrollments");
    public IMongoCollection<TeacherAssignment> TeacherAssignments => _database.GetCollection<TeacherAssignment>("teacherAssignments");
    public IMongoCollection<Assignment> Assignments => _database.GetCollection<Assignment>("assignments");
    public IMongoCollection<Submission> Submissions => _database.GetCollection<Submission>("submissions");
    public IMongoCollection<ApplicationSetting> Settings => _database.GetCollection<ApplicationSetting>("settings");
}