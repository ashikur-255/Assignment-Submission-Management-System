using AssignmentManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AssignmentManagementSystem.Infrastructure.Services;

public sealed class FileStorageService(IWebHostEnvironment environment,IConfiguration configuration):IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions=new(StringComparer.OrdinalIgnoreCase){".pdf",".doc",".docx",".txt",".jpg",".jpeg",".png",".webp"};
    private const long DefaultMaxBytes=10*1024*1024;

    public bool IsAllowed(IFormFile file)
    {
        if(file is null || file.Length==0) return false;
        var ext=Path.GetExtension(file.FileName);
        var max=configuration.GetValue<long?>("FileUpload:MaxBytes")??DefaultMaxBytes;
        return AllowedExtensions.Contains(ext) && file.Length<=max;
    }

    public async Task<string> SaveAsync(IFormFile file,string folder,CancellationToken ct)
    {
        if(!IsAllowed(file)) throw new ArgumentException("Invalid file type or file size.");
        var root=Path.Combine(environment.WebRootPath??Path.Combine(environment.ContentRootPath,"wwwroot"),"uploads",folder);
        Directory.CreateDirectory(root);
        var ext=Path.GetExtension(file.FileName).ToLowerInvariant();
        var name=$"{Guid.NewGuid():N}{ext}";
        var path=Path.Combine(root,name);
        await using var stream=new FileStream(path,FileMode.CreateNew);
        await file.CopyToAsync(stream,ct);
        return $"/uploads/{folder}/{name}";
    }
}