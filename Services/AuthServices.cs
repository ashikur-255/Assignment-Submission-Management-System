using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssignmentManagementSystem.Infrastructure.Services;

public sealed class PasswordService : IPasswordService
{
    public string Hash(string password)=>BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password,string hash)=>BCrypt.Net.BCrypt.Verify(password,hash);
}

public sealed class JwtService(IConfiguration configuration) : IJwtService
{
    public (string Token,DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var s=configuration.GetSection("Jwt");
        var key=s["Key"]??throw new InvalidOperationException("Jwt:Key is missing.");
        var issuer=s["Issuer"]??"AssignmentManagementSystem";
        var audience=s["Audience"]??"AssignmentManagementSystem.Client";
        var minutes=int.TryParse(s["AccessTokenMinutes"],out var m)?m:30;
        var expires=DateTime.UtcNow.AddMinutes(minutes);
        var claims=new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,user.Id),
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}".Trim()),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.Role,user.Role)
        };
        var credentials=new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256);
        var token=new JwtSecurityToken(issuer,audience,claims,expires:expires,signingCredentials:credentials);
        return (new JwtSecurityTokenHandler().WriteToken(token),expires);
    }
    public string GenerateRefreshToken()=>Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public string HashRefreshToken(string token)
    {
        using var sha=SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }
}