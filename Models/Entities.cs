using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssignmentManagementSystem.Core.Models;

public abstract class MongoEntity
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class User : MongoEntity
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = Roles.Student;
    public bool IsActive { get; set; } = true;
}

public sealed class RefreshToken : MongoEntity
{
    public string UserId { get; set; } = "";
    public string TokenHash { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public bool IsRevoked => RevokedAt.HasValue;
}

public sealed class ClassRoom : MongoEntity
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class Course : MongoEntity
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
    public string ClassId { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class Subject : MongoEntity
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
    public string CourseId { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class StudentEnrollment : MongoEntity
{
    public string StudentId { get; set; } = "";
    public string ClassId { get; set; } = "";
    public string CourseId { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class TeacherAssignment : MongoEntity
{
    public string TeacherId { get; set; } = "";
    public string ClassId { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string SubjectId { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public sealed class Assignment : MongoEntity
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string TeacherId { get; set; } = "";
    public string ClassId { get; set; } = "";
    public string CourseId { get; set; } = "";
    public string SubjectId { get; set; } = "";
    public DateTime Deadline { get; set; }
    public decimal MaximumMarks { get; set; }
    public string Status { get; set; } = AssignmentStatuses.Draft;
    public bool AllowUpdateBeforeDeadline { get; set; } = true;
    public string? AttachmentUrl { get; set; }
}

public sealed class Submission : MongoEntity
{
    public string AssignmentId { get; set; } = "";
    public string StudentId { get; set; } = "";
    public string Answer { get; set; } = "";
    public string? AttachmentUrl { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = SubmissionStatuses.Submitted;
    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }
    public string? GradedBy { get; set; }
}

public sealed class ApplicationSetting : MongoEntity
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
    public string Description { get; set; } = "";
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
}

public static class AssignmentStatuses
{
    public const string Draft = "Draft";
    public const string Published = "Published";
    public const string Closed = "Closed";
}

public static class SubmissionStatuses
{
    public const string Submitted = "Submitted";
    public const string Late = "Late";
    public const string Graded = "Graded";
    public const string Returned = "Returned";
}