namespace PontusGo.Application.DTOs;

public class RedemptionValidationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public RedemptionDto? Redemption { get; set; }
}
