namespace PontusGo.Application.DTOs;

public class DailyPointsSummaryDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int PointsEarnedToday { get; set; }
    public int RemainingPointsToday { get; set; }
    public int MaxDailyPoints { get; set; } = 30;
    public IEnumerable<string> ActivitiesCompletedToday { get; set; } = [];
}
