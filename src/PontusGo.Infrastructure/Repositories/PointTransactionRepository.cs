using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;
using PontusGo.Infrastructure.Data;

namespace PontusGo.Infrastructure.Repositories;

public class PointTransactionRepository : IPointTransactionRepository
{
    private readonly PontusGoDbContext _context;

    public PointTransactionRepository(PontusGoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PointTransaction>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.PointTransactions
            .Where(pt => pt.StudentId == studentId)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PointTransaction transaction)
    {
        await _context.PointTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
}