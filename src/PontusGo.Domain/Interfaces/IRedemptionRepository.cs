using PontusGo.Domain.Enums;
using PontusGo.Domain.Models;

namespace PontusGo.Domain.Interfaces;

public interface IRedemptionRepository
{
    Task<Redemption?> GetByIdAsync(Guid id);
    Task<Redemption?> GetByVoucherCodeAsync(string voucherCode);
    Task<IEnumerable<Redemption>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Redemption>> GetAllAsync(RedemptionStatus? status = null);
    Task AddAsync(Redemption redemption);
    Task UpdateAsync(Redemption redemption);
}
