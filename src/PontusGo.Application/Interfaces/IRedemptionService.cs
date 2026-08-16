using PontusGo.Application.DTOs;

namespace PontusGo.Application.Interfaces;

public interface IRedemptionService
{
    Task<RedemptionResultDto> RedeemProductAsync(Guid studentId, Guid productId);
    Task<IEnumerable<RedemptionDto>> GetStudentRedemptionsAsync(Guid studentId);
    Task<IEnumerable<RedemptionDto>> GetAllAsync(string? status = null);
    Task<RedemptionValidationResultDto> ValidateAsync(string voucherCode, Guid adminId);
}
