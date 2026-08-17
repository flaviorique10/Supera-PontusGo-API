using PontusGo.Application.DTOs;
using PontusGo.Domain.Enums;

namespace PontusGo.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllStudentsAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<StudentProfileDto?> GetStudentProfileAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> CreateStudentAsync(CreateStudentDto dto);
    Task<UserDto?> AddPointsAsync(Guid studentId, int points, string description);
    Task<UserDto?> AwardDailyPointsAsync(Guid studentId, AwardDailyPointsDto dto);
    Task<UserDto?> UpdateTuitionStatusAsync(Guid studentId, TuitionStatus status);
    Task<DailyPointsSummaryDto?> GetDailyPointsSummaryAsync(Guid studentId);
}
