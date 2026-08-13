using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/settings"),Authorize(Roles=Roles.Admin)]
public sealed class SettingsController(IRepository<ApplicationSetting> repo):ControllerBase
{
    [HttpGet] public async Task<IActionResult>Get(CancellationToken ct)=>Ok(new ApiResponse<IReadOnlyList<ApplicationSetting>>(true,"Settings retrieved.",await repo.GetAllAsync(ct)));
    [HttpPut("{key}")] public async Task<IActionResult>Put(string key,UpdateSettingRequest r,CancellationToken ct)
    {
        var x=(await repo.GetAllAsync(ct)).FirstOrDefault(s=>s.Key==key);
        if(x is null)x=await repo.InsertAsync(new ApplicationSetting{Key=key,Value=r.Value,Description=r.Description??""},ct);
        else{x.Value=r.Value;x.Description=r.Description??x.Description;await repo.UpdateAsync(x,ct);}
        return Ok(new ApiResponse<ApplicationSetting>(true,"Setting saved.",x));
    }
}