using AssignmentManagementSystem.Core.DTOs;
using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentManagementSystem.API.Controllers;

[ApiController,Route("api/uploads"),Authorize]
public sealed class UploadController(IFileStorageService storage):ControllerBase
{
    [HttpPost("assignment"),Authorize(Roles=Roles.Teacher)]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult>Assignment(IFormFile file,CancellationToken ct)=>Ok(new ApiResponse<string>(true,"Assignment file uploaded.",await storage.SaveAsync(file,"assignments",ct)));

    [HttpPost("submission"),Authorize(Roles=Roles.Student)]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult>Submission(IFormFile file,CancellationToken ct)=>Ok(new ApiResponse<string>(true,"Submission file uploaded.",await storage.SaveAsync(file,"submissions",ct)));
}