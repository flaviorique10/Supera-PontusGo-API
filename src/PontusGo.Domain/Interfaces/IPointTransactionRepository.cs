using PontusGo.Domain.Models;

namespace PontusGo.Domain.Interfaces
{
    public interface IPointTransactionRepository
    {
        Task<IEnumerable<PointTransaction>> GetByStudentIdAsync(Guid studentId);
        Task AddAsync(PointTransaction transaction);
    }
}
