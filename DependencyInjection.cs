using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;
using AssignmentManagementSystem.Infrastructure.Configuration;
using AssignmentManagementSystem.Infrastructure.Data;
using AssignmentManagementSystem.Infrastructure.Repositories;
using AssignmentManagementSystem.Infrastructure.Seed;
using AssignmentManagementSystem.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssignmentManagementSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection("MongoDb"));
        services.AddSingleton<MongoContext>();
        services.AddScoped(typeof(IRepository<>),typeof(MongoRepository<>));
        services.AddScoped<IPasswordService,PasswordService>();
        services.AddScoped<IJwtService,JwtService>();
        services.AddScoped<ICurrentUser,CurrentUser>();
        services.AddScoped<IFileStorageService,FileStorageService>();
        services.AddScoped<IAssignmentService,AssignmentService>();
        services.AddScoped<DatabaseSeeder>();
        return services;
    }
}