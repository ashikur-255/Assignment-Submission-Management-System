using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/auth")]
public sealed class AuthController(IRepository<User> users,IRepository<RefreshToken> refreshTokens,IPasswordService passwords,IJwtService jwt):ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
    RegisterRequest request,
    CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // Check duplicate email
        var usersList = await users.GetAllAsync(ct);

        var emailExists = usersList.Any(
            x => x.Email.Equals(
                email,
                StringComparison.OrdinalIgnoreCase));

        if (emailExists)
        {
            return Conflict(
                new ApiResponse<object>(
                    false,
                    "Email is already registered."));
        }

        // Create student
        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Phone = request.Phone?.Trim() ?? string.Empty,
            PasswordHash = passwords.Hash(request.Password),
            Role = Roles.Student,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await users.InsertAsync(user, ct);

        return Ok(
            new ApiResponse<UserResponse>(
                true,
                "Registration successful.",
                ToResponse(createdUser)));
    }

    [HttpPost("login"),AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest r,CancellationToken ct)
    {
        var u=(await users.GetAllAsync(ct)).FirstOrDefault(x=>x.Email.Equals(r.Email,StringComparison.OrdinalIgnoreCase));
        if(u is null||!passwords.Verify(r.Password,u.PasswordHash))return Unauthorized(new ApiResponse<object>(false,"Invalid email or password."));
        if(!u.IsActive)return Unauthorized(new ApiResponse<object>(false,"Your account is inactive."));
        return Ok(new ApiResponse<AuthResponse>(true,"Login successful.",await IssueTokens(u,ct)));
    }

    [HttpPost("refresh"),AllowAnonymous]
    public async Task<IActionResult> Refresh(RefreshTokenRequest r,CancellationToken ct)
    {
        var hash=jwt.HashRefreshToken(r.RefreshToken);
        var stored=(await refreshTokens.GetAllAsync(ct)).FirstOrDefault(x=>x.TokenHash==hash);
        if(stored is null||stored.IsRevoked||stored.ExpiresAt<=DateTime.UtcNow)return Unauthorized(new ApiResponse<object>(false,"Invalid or expired refresh token."));
        var u=await users.GetByIdAsync(stored.UserId,ct);
        if(u is null||!u.IsActive)return Unauthorized(new ApiResponse<object>(false,"User account is unavailable."));
        stored.RevokedAt=DateTime.UtcNow;
        var auth=await IssueTokens(u,ct);
        stored.ReplacedByTokenHash=jwt.HashRefreshToken(auth.RefreshToken);
        await refreshTokens.UpdateAsync(stored,ct);
        return Ok(new ApiResponse<AuthResponse>(true,"Token refreshed.",auth));
    }

    [HttpPost("logout"),Authorize]
    public async Task<IActionResult> Logout(RefreshTokenRequest r,CancellationToken ct)
    {
        var hash=jwt.HashRefreshToken(r.RefreshToken);
        var stored=(await refreshTokens.GetAllAsync(ct)).FirstOrDefault(x=>x.TokenHash==hash);
        if(stored is not null&&!stored.IsRevoked){stored.RevokedAt=DateTime.UtcNow;await refreshTokens.UpdateAsync(stored,ct);}
        return Ok(new ApiResponse<object>(true,"Logged out successfully."));
    }

    [HttpGet("me"),Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var id=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var u=await users.GetByIdAsync(id??"",ct);
        return u is null?NotFound(new ApiResponse<object>(false,"User not found.")):Ok(new ApiResponse<UserResponse>(true,"Current user.",ToResponse(u)));
    }

    private async Task<AuthResponse> IssueTokens(User u,CancellationToken ct)
    {
        var access=jwt.GenerateAccessToken(u);
        var token=jwt.GenerateRefreshToken();
        var days=30;
        var refreshExpiry=DateTime.UtcNow.AddDays(days);
        await refreshTokens.InsertAsync(new RefreshToken{UserId=u.Id,TokenHash=jwt.HashRefreshToken(token),ExpiresAt=refreshExpiry},ct);
        return new AuthResponse(access.Token,token,ToResponse(u),access.ExpiresAt,refreshExpiry);
    }
    private static UserResponse ToResponse(User x)=>new(x.Id,x.FirstName,x.LastName,x.Email,x.Phone,x.Role,x.IsActive,x.CreatedAt);
}