using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;

namespace PontusGo.Application.Services;

public class UserService : IUserService
{
    public const int MaxDailyPoints = 30;
    public const int PointsPerActivity = 10;

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
        var students = (await _userRepository.GetAllStudentsAsync()).ToList();
        var studentDtos = new List<UserDto>();

        foreach (var student in students)
        {
            var pointsToday = await _transactionRepository.GetPointsAwardedOnDateAsync(student.Id, DateTime.UtcNow);
            studentDtos.Add(MapToDto(student, pointsToday));
        }

        return studentDtos;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        var pointsToday = user.Role == UserRole.Student
            ? await _transactionRepository.GetPointsAwardedOnDateAsync(user.Id, DateTime.UtcNow)
            : 0;

        return MapToDto(user, pointsToday);
    }

    public async Task<StudentProfileDto?> GetStudentProfileAsync(Guid id)
    {
        var student = await _userRepository.GetByIdAsync(id);
        if (student == null || student.Role != UserRole.Student) return null;

        var redemptions = (await _redemptionRepository.GetByStudentIdAsync(id)).ToList();
        var pointsToday = await _transactionRepository.GetPointsAwardedOnDateAsync(student.Id, DateTime.UtcNow);
        var remainingPointsToday = Math.Max(0, MaxDailyPoints - pointsToday);

        return new StudentProfileDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Role = student.Role.ToString(),
            TuitionStatus = student.TuitionStatus.ToString(),
            TotalPoints = student.TotalPoints,
            PointsEarnedToday = pointsToday,
            RemainingPointsToday = remainingPointsToday,
            MaxDailyPoints = MaxDailyPoints,
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

        if (!Enum.IsDefined(typeof(TuitionStatus), dto.TuitionStatus))
            dto.TuitionStatus = TuitionStatus.UpToDate;

        return await CreateUserAsync(dto.Name, dto.Email, dto.Password, (UserRole)dto.RoleId, dto.TuitionStatus);
    }

    public Task<UserDto> CreateStudentAsync(CreateStudentDto dto)
    {
        if (!Enum.IsDefined(typeof(TuitionStatus), dto.TuitionStatus))
            dto.TuitionStatus = TuitionStatus.UpToDate;

        return CreateUserAsync(dto.Name, dto.Email, dto.Password, UserRole.Student, dto.TuitionStatus);
    }

    public async Task<UserDto?> AddPointsAsync(Guid studentId, int points, string description)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        if (student == null || student.Role != UserRole.Student) return null;

        if (points <= 0)
            throw new ArgumentException("A quantidade de pontos deve ser maior que zero.");

        if (points > MaxDailyPoints)
            throw new ArgumentException($"A quantidade de pontos por bonificação não pode exceder o limite diário de {MaxDailyPoints} pontos.");

        var pointsEarnedToday = await _transactionRepository.GetPointsAwardedOnDateAsync(student.Id, DateTime.UtcNow);
        if (pointsEarnedToday + points > MaxDailyPoints)
        {
            var remaining = Math.Max(0, MaxDailyPoints - pointsEarnedToday);
            throw new InvalidOperationException(
                $"Limite diário excedido. O aluno pode ganhar no máximo {MaxDailyPoints} pontos por dia. Hoje já foram concedidos {pointsEarnedToday} pontos (restam {remaining} pontos disponíveis).");
        }

        student.AddPoints(points);
        var transaction = new PointTransaction(student.Id, points, description.Trim())
        {
            PointsAwarded = points,
            ActivityDescription = description.Trim()
        };

        await _transactionRepository.AddAsync(transaction);
        await _userRepository.UpdateAsync(student);

        var newPointsToday = pointsEarnedToday + points;
        return MapToDto(student, newPointsToday);
    }

    public async Task<UserDto?> AwardDailyPointsAsync(Guid studentId, AwardDailyPointsDto dto)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        if (student == null || student.Role != UserRole.Student) return null;

        var selectedActivities = new List<string>();
        int totalPoints = 0;

        if (dto.Assiduidade)
        {
            selectedActivities.Add("Assiduidade (+10 pts)");
            totalPoints += PointsPerActivity;
        }

        if (dto.Participacao)
        {
            selectedActivities.Add("Participação (+10 pts)");
            totalPoints += PointsPerActivity;
        }

        if (dto.FazerTarefa)
        {
            selectedActivities.Add("Fazer Tarefa (+10 pts)");
            totalPoints += PointsPerActivity;
        }

        if (totalPoints == 0)
        {
            throw new ArgumentException("Selecione ao menos uma atividade (Assiduidade, Participação ou Fazer Tarefa) para conceder pontos.");
        }

        var description = string.Join(", ", selectedActivities);
        if (!string.IsNullOrWhiteSpace(dto.Observation))
        {
            description += $" - Obs: {dto.Observation.Trim()}";
        }

        return await AddPointsAsync(studentId, totalPoints, description);
    }

    public async Task<UserDto?> UpdateTuitionStatusAsync(Guid studentId, TuitionStatus status)
    {
        if (!Enum.IsDefined(typeof(TuitionStatus), status))
            throw new ArgumentException("Status de mensalidade inválido.");

        var student = await _userRepository.GetByIdAsync(studentId);
        if (student == null || student.Role != UserRole.Student) return null;

        student.UpdateTuitionStatus(status);
        await _userRepository.UpdateAsync(student);

        var pointsToday = await _transactionRepository.GetPointsAwardedOnDateAsync(student.Id, DateTime.UtcNow);
        return MapToDto(student, pointsToday);
    }

    public async Task<DailyPointsSummaryDto?> GetDailyPointsSummaryAsync(Guid studentId)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        if (student == null || student.Role != UserRole.Student) return null;

        var todayTransactions = (await _transactionRepository.GetTransactionsOnDateAsync(student.Id, DateTime.UtcNow)).ToList();
        var pointsEarnedToday = todayTransactions.Sum(t => t.PointsAwarded);
        var remainingPointsToday = Math.Max(0, MaxDailyPoints - pointsEarnedToday);

        return new DailyPointsSummaryDto
        {
            StudentId = student.Id,
            StudentName = student.Name,
            PointsEarnedToday = pointsEarnedToday,
            RemainingPointsToday = remainingPointsToday,
            MaxDailyPoints = MaxDailyPoints,
            ActivitiesCompletedToday = todayTransactions.Select(t => t.ActivityDescription)
        };
    }

    private async Task<UserDto> CreateUserAsync(string name, string email, string password, UserRole role, TuitionStatus tuitionStatus)
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
            Role = role,
            TuitionStatus = tuitionStatus
        };

        await _userRepository.AddAsync(user);
        return MapToDto(user, 0);
    }

    private static UserDto MapToDto(User user, int pointsEarnedToday = 0)
    {
        var remainingPoints = Math.Max(0, MaxDailyPoints - pointsEarnedToday);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            TuitionStatus = user.TuitionStatus.ToString(),
            TotalPoints = user.TotalPoints,
            PointsEarnedToday = pointsEarnedToday,
            RemainingPointsToday = remainingPoints,
            MaxDailyPoints = MaxDailyPoints
        };
    }
}
