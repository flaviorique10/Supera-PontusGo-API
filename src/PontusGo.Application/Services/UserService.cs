using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;

namespace PontusGo.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPointTransactionRepository _transactionRepository;
    private readonly IRedemptionRepository _redemptionRepository;

    public UserService(
        IUserRepository userRepository,
        IPointTransactionRepository transactionRepository,
        IRedemptionRepository redemptionRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _redemptionRepository = redemptionRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllStudentsAsync()
    {
        var students = await _userRepository.GetAllStudentsAsync();
        return students.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<StudentProfileDto?> GetStudentProfileAsync(Guid id)
    {
        var student = await _userRepository.GetByIdAsync(id);
        if (student == null || student.Role != UserRole.Student) return null;

        var redemptions = (await _redemptionRepository.GetByStudentIdAsync(id)).ToList();
        return new StudentProfileDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Role = student.Role.ToString(),
            TotalPoints = student.TotalPoints,
            TotalRedemptions = redemptions.Count,
            PendingRedemptions = redemptions.Count(r => r.Status == RedemptionStatus.Pending),
            CollectedRedemptions = redemptions.Count(r => r.Status == RedemptionStatus.Collected),
            RecentRedemptions = redemptions.Take(10).Select(RedemptionMapper.ToDto)
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (!Enum.IsDefined(typeof(UserRole), dto.RoleId))
            throw new ArgumentException("Perfil de usuário inválido.");

        return await CreateUserAsync(dto.Name, dto.Email, dto.Password, (UserRole)dto.RoleId);
    }

    public Task<UserDto> CreateStudentAsync(CreateStudentDto dto)
    {
        return CreateUserAsync(dto.Name, dto.Email, dto.Password, UserRole.Student);
    }

    public async Task<UserDto?> AddPointsAsync(Guid studentId, int points, string description)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        if (student == null || student.Role != UserRole.Student) return null;

        student.AddPoints(points);
        var transaction = new PointTransaction(student.Id, points, description.Trim())
        {
            PointsAwarded = points,
            ActivityDescription = description.Trim()
        };

        await _transactionRepository.AddAsync(transaction);
        await _userRepository.UpdateAsync(student);
        return MapToDto(student);
    }

    private async Task<UserDto> CreateUserAsync(string name, string email, string password, UserRole role)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await _userRepository.GetByEmailAsync(normalizedEmail) != null)
            throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");

        if (password.Length < 8)
            throw new ArgumentException("A senha deve ter pelo menos 8 caracteres.");

        var user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };

        await _userRepository.AddAsync(user);
        return MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            TotalPoints = user.TotalPoints
        };
    }
}
