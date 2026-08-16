using PontusGo.Domain.Models;

namespace PontusGo.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllStudentsAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
