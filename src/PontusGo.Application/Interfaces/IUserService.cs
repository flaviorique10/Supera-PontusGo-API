using PontusGo.Application.DTOs;

namespace PontusGo.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllStudentsAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<StudentProfileDto?> GetStudentProfileAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> CreateStudentAsync(CreateStudentDto dto);
    Task<UserDto?> AddPointsAsync(Guid studentId, int points, string description);
}
