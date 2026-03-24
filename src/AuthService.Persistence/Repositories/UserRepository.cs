using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
 
namespace AuthService.Persistence.Repositories;
 
public class UserRepository(ApplicationDbContext context):IUserRepository
{
    private readonly ApplicationDbContext _context = context;
 
    public async Task<User?> GetByIdAsync(string id)
    {
        //Sirve para cargar todos los datos relacionados con el usuario
       var user = await _context.Users
       .Include(u => u.UserProfile)
       .Include(u => u.UserEmail)
       .Include(u => u.UserRoles)
       .Include(u => u.UserPasswordReset)
       .FirstOrDefaultAsync(u => u.Id == id);
 
       return user ?? throw new InvalidOperationException($"Usuario con id {id} no encontrado");
    }
 
    public async Task<User?> GetByEmailAsync(string email)
    {
        //Sirve para cargar todos los datos relacionados con el usuario
       var user = await _context.Users
       .Include(u => u.UserProfile)
       .Include(u => u.UserEmail)
       .Include(u => u.UserRoles)
       .Include(u => u.UserPasswordReset)
       .FirstOrDefaultAsync(u => EF.Functions.Like(u.Email, email));
 
       return user ?? throw new InvalidOperationException($"Usuario con email {email} no encontrado");
    }
 
    public async Task<User?> GetByUsernameAsync(string username)
    {
        //Sirve para cargar todos los datos relacionados con el usuario
       var user = await _context.Users
       .Include(u => u.UserProfile)
       .Include(u => u.UserEmail)
       .Include(u => u.UserRoles)
       .Include(u => u.UserPasswordReset)
       .FirstOrDefaultAsync(u => EF.Functions.Like(u.Username, username));
 
       return user ?? throw new InvalidOperationException($"Usuario con username {username} no encontrado");
    }
    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        var user = await _context.Users
       .Include(u => u.UserProfile)
       .Include(u => u.UserEmail)
       .Include(u => u.UserRoles)
       .Include(u => u.UserPasswordReset)
       .FirstOrDefaultAsync(u => u.UserEmail != null && u.UserEmail.EmailVerificationToken == token);
 
       return user;
    }
 
    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        var user = await _context.Users
       .Include(u => u.UserProfile)
       .Include(u => u.UserEmail)
       .Include(u => u.UserRoles)
       .Include(u => u.UserPasswordReset)
       .FirstOrDefaultAsync(u => u.UserPasswordReset != null && u.UserPasswordReset.PasswordResetToken == token);
 
       return user;
    }
    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }
 
    public async Task<User> UpdateAsync(User user)
    {
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }
 
    public async Task<User> DeleteAsync(User user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return user;
    }
 
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
       .AnyAsync(u => EF.Functions.Like(u.Email, email));
    }
 
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users
       .AnyAsync(u => EF.Functions.Like(u.Username, username));
    }
    public async Task UpdateUserRoleAsync(string userId, string roleId)
    {
        var existingRoles = await _context.UserRoles
       .Where(ur => ur.UserId == userId)
       .ToListAsync();
       
       context.UserRoles.RemoveRange(existingRoles);
       
       var newUserRole = new UserRole
       {
        Id = Guid.NewGuid().ToString("N")[..16],        
        UserId = userId,
        RoleId = roleId,
       };
       context.UserRoles.Add(newUserRole);
       await context.SaveChangesAsync();
    }
}
 