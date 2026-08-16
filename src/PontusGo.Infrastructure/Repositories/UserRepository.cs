using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;
using PontusGo.Infrastructure.Data;

namespace PontusGo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PontusGoDbContext _context;

    public UserRepository(PontusGoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _context.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == normalizedEmail);
    }

    public async Task<IEnumerable<User>> GetAllStudentsAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(user => user.Role == UserRole.Student)
            .OrderBy(user => user.Name)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
