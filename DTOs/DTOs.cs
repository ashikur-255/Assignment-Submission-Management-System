using System.ComponentModel.DataAnnotations;

namespace AssignmentManagementSystem.Core.DTOs;

// ============================================================
// AUTHENTICATION / USER DTOs
// ============================================================

public sealed record RegisterRequest(
    [param: Required, MaxLength(80)]
    string FirstName,

    [param: Required, MaxLength(80)]
    string LastName,

    [param: Required, EmailAddress, MaxLength(255)]
    string Email,

    [param: MaxLength(30)]
    string? Phone,

    [param: Required, MinLength(8), MaxLength(100)]
    string Password
);

public sealed record AdminCreateUserRequest(
    [param: Required, MaxLength(80)]
    string FirstName,

    [param: Required, MaxLength(80)]
    string LastName,

    [param: Required, EmailAddress, MaxLength(255)]
    string Email,

    [param: MaxLength(30)]
    string? Phone,

    [param: Required, MinLength(8), MaxLength(100)]
    string Password,

    [param: Required]
    string Role
);

public sealed record AdminUpdateUserRequest(
    [param: Required, MaxLength(80)]
    string FirstName,

    [param: Required, MaxLength(80)]
    string LastName,

    [param: MaxLength(30)]
    string? Phone,

    [param: Required]
    string Role,

    bool IsActive
);

public sealed record LoginRequest(
    [param: Required, EmailAddress, MaxLength(255)]
    string Email,

    [param: Required]
    string Password
);

public sealed record RefreshTokenRequest(
    [param: Required]
    string RefreshToken
);


// ============================================================
// CLASS DTOs
// ============================================================

public sealed record CreateClassRequest(
    [param: Required, MaxLength(120)]
    string Name,

    [param: Required, MaxLength(30)]
    string Code,

    string? Description
);

public sealed record UpdateClassRequest(
    [param: Required, MaxLength(120)]
    string Name,

    [param: Required, MaxLength(30)]
    string Code,

    string? Description,

    bool IsActive
);


// ============================================================
// COURSE DTOs
// ============================================================

public sealed record CreateCourseRequest(
    [param: Required, MaxLength(120)]
    string Name,

    [param: Required, MaxLength(30)]
    string Code,

    string? Description,

    [param: Required]
    string ClassId
);

public sealed record UpdateCourseRequest(
    [param: Required, MaxLength(120)]
    string Name,

    [param: Required, MaxLength(30)]
    string Code,

    string? Description,

    [param: Required]
    string ClassId,

    bool IsActive
);


// ============================================================
// SUBJECT DTOs
// ============================================================

public sealed record CreateSubjectRequest(
    [param: Required, MaxLength(120)]
    string Name,

    [param: Required, MaxLength(30)]
    string Code,

    string? Description,

    [param: Required]
    string CourseId
);

public sealed record UpdateSubjectRequest(
    [param: Required, MaxLength(120)]
    string Name,

    [param: Required, MaxLength(30)]
    string Code,

    string? Description,

    [param: Required]
    string CourseId,

    bool IsActive
);


// ============================================================
// ENROLLMENT DTOs
// ============================================================

public sealed record CreateEnrollmentRequest(
    [param: Required]
    string StudentId,

    [param: Required]
    string ClassId,

    [param: Required]
    string CourseId
);

public sealed record UpdateEnrollmentRequest(
    [param: Required]
    string ClassId,

    [param: Required]
    string CourseId,

    bool IsActive
);


// ============================================================
// TEACHER ASSIGNMENT DTO
// ============================================================

public sealed record AssignTeacherRequest(
    [param: Required]
    string TeacherId,

    [param: Required]
    string ClassId,

    [param: Required]
    string CourseId,

    [param: Required]
    string SubjectId
);


// ============================================================
// ASSIGNMENT DTOs
// ============================================================

public sealed record CreateAssignmentRequest(
    [param: Required, MaxLength(200)]
    string Title,

    [param: Required]
    string Description,

    [param: Required]
    string ClassId,

    [param: Required]
    string CourseId,

    [param: Required]
    string SubjectId,

    DateTime Deadline,

    [param: Range(0.01, 100000)]
    decimal MaximumMarks,

    bool AllowUpdateBeforeDeadline,

    string? AttachmentUrl
);

public sealed record UpdateAssignmentRequest(
    [param: Required, MaxLength(200)]
    string Title,

    [param: Required]
    string Description,

    [param: Required]
    string ClassId,

    [param: Required]
    string CourseId,

    [param: Required]
    string SubjectId,

    DateTime Deadline,

    [param: Range(0.01, 100000)]
    decimal MaximumMarks,

    bool AllowUpdateBeforeDeadline,

    string? AttachmentUrl
);


// ============================================================
// SUBMISSION DTOs
// ============================================================

public sealed record CreateSubmissionRequest(
    [param: Required]
    string AssignmentId,

    string Answer,

    string? AttachmentUrl
);

public sealed record UpdateSubmissionRequest(
    string Answer,

    string? AttachmentUrl
);

public sealed record GradeSubmissionRequest(
    [param: Range(0, double.MaxValue)]
    decimal Marks,

    string? Feedback
);

public sealed record ChangeSubmissionStatusRequest(
    [param: Required]
    string Status
);


// ============================================================
// SETTINGS DTO
// ============================================================

public sealed record UpdateSettingRequest(
    [param: Required]
    string Value,

    string? Description
);


// ============================================================
// GENERIC API RESPONSES
// ============================================================

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default
);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    long Total,
    int Page,
    int PageSize
);


// ============================================================
// RESPONSE DTOs
// ============================================================

public sealed record UserResponse(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    UserResponse User,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt
);