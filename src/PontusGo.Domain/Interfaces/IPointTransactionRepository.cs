using PontusGo.Domain.Models;

namespace PontusGo.Domain.Interfaces
{
    public interface IPointTransactionRepository
    {
        Task<IEnumerable<PointTransaction>> GetByStudentIdAsync(Guid studentId);
        Task<int> GetPointsAwardedOnDateAsync(Guid studentId, DateTime dateUtc);
        Task<IEnumerable<PointTransaction>> GetTransactionsOnDateAsync(Guid studentId, DateTime dateUtc);
        Task AddAsync(PointTransaction transaction);
    }
}
