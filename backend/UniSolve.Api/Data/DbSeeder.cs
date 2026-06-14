using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Data;
using UniSolve.Api.Models.Entities;
using UniSolve.Api.Models.Enums;

namespace UniSolve.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Subjects.AnyAsync())
            return;

        var subjects = new[]
        {
            new Subject { Name = "Mathematics", Code = "MATH" },
            new Subject { Name = "Physics", Code = "PHYS" },
            new Subject { Name = "Computer Science", Code = "CS" },
            new Subject { Name = "Chemistry", Code = "CHEM" },
            new Subject { Name = "Engineering", Code = "ENG" }
        };
        context.Subjects.AddRange(subjects);

        var admin = new User
        {
            Email = "admin@unisolve.com",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin
        };
        context.Users.Add(admin);

        var verified = new User
        {
            Email = "tutor@unisolve.com",
            Username = "tutor",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tutor123!"),
            Role = UserRole.Verified
        };
        context.Users.Add(verified);

        await context.SaveChangesAsync();
    }
}
