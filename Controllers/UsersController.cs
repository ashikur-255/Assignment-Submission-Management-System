using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/users"),Authorize(Roles=Roles.Admin)]
public sealed class UsersController(IRepository<User> users,IPasswordService passwords):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]string? search,[FromQuery]string? role,[FromQuery]bool? active,[FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);
        var q=(await users.GetAllAsync(ct)).AsEnumerable();
        if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>$"{x.FirstName} {x.LastName} {x.Email} {x.Phone}".Contains(search,StringComparison.OrdinalIgnoreCase));
        if(!string.IsNullOrWhiteSpace(role))q=q.Where(x=>x.Role.Equals(role,StringComparison.OrdinalIgnoreCase));
        if(active.HasValue)q=q.Where(x=>x.IsActive==active.Value);
        var total=q.LongCount();var data=q.Skip((page-1)*pageSize).Take(pageSize).Select(ToResponse).ToList();
        return Ok(new ApiResponse<PagedResult<UserResponse>>(true,"Users retrieved.",new(data,total,page,pageSize)));
    }

    [HttpPost]
    public async Task<IActionResult>Create(AdminCreateUserRequest r,CancellationToken ct)
    {
        if(!new[]{Roles.Admin,Roles.Teacher,Roles.Student}.Contains(r.Role))throw new ArgumentException("Invalid role.");
        if((await users.GetAllAsync(ct)).Any(x=>x.Email.Equals(r.Email,StringComparison.OrdinalIgnoreCase)))return Conflict(new ApiResponse<object>(false,"Email is already registered."));
        var u=await users.InsertAsync(new User{FirstName=r.FirstName.Trim(),LastName=r.LastName.Trim(),Email=r.Email.Trim().ToLowerInvariant(),Phone=r.Phone??"",PasswordHash=passwords.Hash(r.Password),Role=r.Role},ct);
        return Ok(new ApiResponse<UserResponse>(true,"User created.",ToResponse(u)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Update(string id,AdminUpdateUserRequest r,CancellationToken ct)
    {
        if(!new[]{Roles.Admin,Roles.Teacher,Roles.Student}.Contains(r.Role))throw new ArgumentException("Invalid role.");
        var u=await users.GetByIdAsync(id,ct)??throw new KeyNotFoundException("User not found.");
        u.FirstName=r.FirstName.Trim();u.LastName=r.LastName.Trim();u.Phone=r.Phone??"";u.Role=r.Role;u.IsActive=r.IsActive;
        await users.UpdateAsync(u,ct);return Ok(new ApiResponse<UserResponse>(true,"User updated.",ToResponse(u)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>GetById(string id,CancellationToken ct){var u=await users.GetByIdAsync(id,ct);return u is null?NotFound(new ApiResponse<object>(false,"User not found.")):Ok(new ApiResponse<UserResponse>(true,"User retrieved.",ToResponse(u)));}

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(string id,CancellationToken ct){await users.DeleteAsync(id,ct);return Ok(new ApiResponse<object>(true,"User deleted."));}

    private static UserResponse ToResponse(User x)=>new(x.Id,x.FirstName,x.LastName,x.Email,x.Phone,x.Role,x.IsActive,x.CreatedAt);
}