using AssignmentManagementSystem.Core.Models;
using MongoDB.Driver;

namespace AssignmentManagementSystem.Infrastructure.Data;

public static class MongoIndexes
{
    public static async Task CreateAsync(MongoContext db)
    {
        // =====================================================
        // USERS
        // =====================================================

        await db.Users.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.Role))
        });


        // =====================================================
        // REFRESH TOKENS
        // =====================================================

        await db.RefreshTokens.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(x => x.TokenHash),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(x => x.UserId))
        });


        // =====================================================
        // CLASSES
        // =====================================================

        await db.Classes.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<ClassRoom>(
                Builders<ClassRoom>.IndexKeys.Ascending(x => x.Code),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<ClassRoom>(
                Builders<ClassRoom>.IndexKeys.Ascending(x => x.Name))
        });


        // =====================================================
        // COURSES
        // =====================================================

        await db.Courses.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Course>(
                Builders<Course>.IndexKeys
                    .Ascending(x => x.ClassId)
                    .Ascending(x => x.Code),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<Course>(
                Builders<Course>.IndexKeys.Ascending(x => x.ClassId))
        });


        // =====================================================
        // SUBJECTS
        // =====================================================

        await db.Subjects.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Subject>(
                Builders<Subject>.IndexKeys
                    .Ascending(x => x.CourseId)
                    .Ascending(x => x.Code),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<Subject>(
                Builders<Subject>.IndexKeys.Ascending(x => x.CourseId))
        });


        // =====================================================
        // ENROLLMENTS
        // =====================================================

        await db.StudentEnrollments.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<StudentEnrollment>(
                Builders<StudentEnrollment>.IndexKeys
                    .Ascending(x => x.StudentId)
                    .Ascending(x => x.CourseId),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<StudentEnrollment>(
                Builders<StudentEnrollment>.IndexKeys
                    .Ascending(x => x.ClassId))
        });


        // =====================================================
        // TEACHER ASSIGNMENTS
        // =====================================================

        await db.TeacherAssignments.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<TeacherAssignment>(
                Builders<TeacherAssignment>.IndexKeys
                    .Ascending(x => x.TeacherId)
                    .Ascending(x => x.ClassId)
                    .Ascending(x => x.CourseId)
                    .Ascending(x => x.SubjectId),
                new CreateIndexOptions
                {
                    Unique = true
                })
        });


        // =====================================================
        // ASSIGNMENTS
        // =====================================================

        await db.Assignments.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Assignment>(
                Builders<Assignment>.IndexKeys
                    .Ascending(x => x.ClassId)
                    .Ascending(x => x.CourseId)
                    .Ascending(x => x.Status)),

            new CreateIndexModel<Assignment>(
                Builders<Assignment>.IndexKeys
                    .Ascending(x => x.TeacherId)
                    .Descending(x => x.CreatedAt))
        });


        // =====================================================
        // SUBMISSIONS
        // =====================================================

        await db.Submissions.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Submission>(
                Builders<Submission>.IndexKeys
                    .Ascending(x => x.AssignmentId)
                    .Ascending(x => x.StudentId),
                new CreateIndexOptions
                {
                    Unique = true
                }),

            new CreateIndexModel<Submission>(
                Builders<Submission>.IndexKeys
                    .Ascending(x => x.StudentId))
        });
    }
}