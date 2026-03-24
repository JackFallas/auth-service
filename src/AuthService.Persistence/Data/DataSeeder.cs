using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        // 1. SEED DE ROLES
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                // Usamos Guid.NewGuid().ToString("N").Substring(0, 16) para cumplir con tu MaxLength(16)
                new() { Id = Guid.NewGuid().ToString("N").Substring(0, 16), Name = "ADMIN" },
                new() { Id = Guid.NewGuid().ToString("N").Substring(0, 16), Name = "USER" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 2. SEED DE USUARIO ADMIN
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "ADMIN");
            if (adminRole != null)
            {
                var userId = Guid.NewGuid().ToString("N").Substring(0, 16);
                
                var adminUser = new User
                {
                    Id = userId,
                    Name = "Admin",
                    Surname = "User",
                    Username = "admin",
                    Email = "admin@ksports.local",
                    Password = new string('x', 255), // Cumple con tu MinLength(255)
                    Status = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserProfile = new UserProfile
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 16),
                        UserId = userId
                    },
                    UserEmail = new UserEmail
                    {
                        Id = Guid.NewGuid().ToString("N").Substring(0, 16),
                        UserId = userId,
                        EmailVerified = true,
                        EmailVerificationToken = "initial-token",
                        // AQUÍ ESTABA EL ERROR: No puede ser NULL. Ponemos una fecha futura.
                        EmailVerificationTokenExpiry = DateTime.UtcNow.AddYears(1) 
                    },
                    UserRoles = new List<UserRole>
                    {
                        new UserRole
                        {
                            Id = Guid.NewGuid().ToString("N").Substring(0, 16),
                            UserId = userId,
                            RoleId = adminRole.Id
                        }
                    }
                };
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}