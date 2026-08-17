namespace PontusGo.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public string TuitionStatus { get; set; } = "UpToDate";
    public int PointsEarnedToday { get; set; }
    public int RemainingPointsToday { get; set; } = 30;
    public int MaxDailyPoints { get; set; } = 30;
}