using AssignmentManagementSystem.Core.Interfaces;
using AssignmentManagementSystem.Core.Models;

namespace AssignmentManagementSystem.Infrastructure.Seed;

public sealed class DatabaseSeeder(IRepository<User> users,IPasswordService passwords)
{
    public async Task SeedAsync()
    {
        var all=await users.GetAllAsync();
        if(all.Any())return;
        await users.InsertAsync(new User{FirstName="System",LastName="Administrator",Email="admin@assignment.local",PasswordHash=passwords.Hash("Admin@12345"),Role=Roles.Admin});
        await users.InsertAsync(new User{FirstName="Demo",LastName="Teacher",Email="teacher@assignment.local",PasswordHash=passwords.Hash("Teacher@12345"),Role=Roles.Teacher});
        await users.InsertAsync(new User{FirstName="Demo",LastName="Student",Email="student@assignment.local",PasswordHash=passwords.Hash("Student@12345"),Role=Roles.Student});
    }
}