namespace PontusGo.Application.DTOs;

public class RedemptionResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RemainingPoints { get; set; }
    public Guid? RedemptionId { get; set; }
    public string? VoucherCode { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
