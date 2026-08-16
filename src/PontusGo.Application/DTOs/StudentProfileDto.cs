namespace PontusGo.Application.DTOs;

public class StudentProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int TotalRedemptions { get; set; }
    public int PendingRedemptions { get; set; }
    public int CollectedRedemptions { get; set; }
    public IEnumerable<RedemptionDto> RecentRedemptions { get; set; } = [];
}
