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

    public async Task<int> GetPointsAwardedOnDateAsync(Guid studentId, DateTime dateUtc)
    {
        var startOfDay = dateUtc.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.PointTransactions
            .Where(pt => pt.StudentId == studentId && pt.CreatedAt >= startOfDay && pt.CreatedAt < endOfDay)
            .SumAsync(pt => (int?)pt.PointsAwarded) ?? 0;
    }

    public async Task<IEnumerable<PointTransaction>> GetTransactionsOnDateAsync(Guid studentId, DateTime dateUtc)
    {
        var startOfDay = dateUtc.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _context.PointTransactions
            .Where(pt => pt.StudentId == studentId && pt.CreatedAt >= startOfDay && pt.CreatedAt < endOfDay)
            .OrderByDescending(pt => pt.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PointTransaction transaction)
    {
        await _context.PointTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
}