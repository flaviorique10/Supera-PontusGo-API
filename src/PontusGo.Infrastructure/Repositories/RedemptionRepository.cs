using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;
using PontusGo.Infrastructure.Data;

namespace PontusGo.Infrastructure.Repositories;

public class RedemptionRepository : IRedemptionRepository
{
    private readonly PontusGoDbContext _context;

    public RedemptionRepository(PontusGoDbContext context)
    {
        _context = context;
    }

    public async Task<Redemption?> GetByIdAsync(Guid id)
    {
        return await Query().FirstOrDefaultAsync(redemption => redemption.Id == id);
    }

    public async Task<Redemption?> GetByVoucherCodeAsync(string voucherCode)
    {
        return await Query().FirstOrDefaultAsync(redemption => redemption.VoucherCode == voucherCode);
    }

    public async Task<IEnumerable<Redemption>> GetByStudentIdAsync(Guid studentId)
    {
        return await Query()
            .Where(redemption => redemption.StudentId == studentId)
            .OrderByDescending(redemption => redemption.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Redemption>> GetAllAsync(RedemptionStatus? status = null)
    {
        var query = Query();
        if (status.HasValue)
            query = query.Where(redemption => redemption.Status == status.Value);

        return await query
            .OrderByDescending(redemption => redemption.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Redemption redemption)
    {
        await _context.Redemptions.AddAsync(redemption);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Redemption redemption)
    {
        _context.Redemptions.Update(redemption);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Redemption> Query()
    {
        return _context.Redemptions
            .Include(redemption => redemption.Product)
            .Include(redemption => redemption.Student);
    }
}
