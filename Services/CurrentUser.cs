using System.Security.Claims;
using AssignmentManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AssignmentManagementSystem.Infrastructure.Services;
public sealed class CurrentUser(IHttpContextAccessor accessor):ICurrentUser
{
    private ClaimsPrincipal User=>accessor.HttpContext?.User??new ClaimsPrincipal();
    public string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??"";
    public string Role=>User.FindFirstValue(ClaimTypes.Role)??"";
    public bool IsAuthenticated=>User.Identity?.IsAuthenticated==true;
}