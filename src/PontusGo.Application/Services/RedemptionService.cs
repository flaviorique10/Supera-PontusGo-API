using System.Security.Cryptography;
using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;
using PontusGo.Domain.Enums;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;

namespace PontusGo.Application.Services;

public class RedemptionService : IRedemptionService
{
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IRedemptionRepository _redemptionRepository;

    public RedemptionService(
        IUserRepository userRepository,
        IProductRepository productRepository,
        IRedemptionRepository redemptionRepository)
    {
        _userRepository = userRepository;
        _productRepository = productRepository;
        _redemptionRepository = redemptionRepository;
    }

    public async Task<RedemptionResultDto> RedeemProductAsync(Guid studentId, Guid productId)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        var product = await _productRepository.GetByIdAsync(productId);

        if (student == null || student.Role != UserRole.Student)
            return new RedemptionResultDto { Success = false, Message = "Aluno não encontrado." };
        if (product == null)
            return new RedemptionResultDto { Success = false, Message = "Produto não encontrado." };
        if (!product.IsActive)
            return new RedemptionResultDto { Success = false, Message = "Produto inativo." };

        try
        {
            student.DeductPoints(product.PointsCost);
            product.DecreaseStock(1);

            var voucherCode = await GenerateUniqueVoucherCodeAsync();
            var redemption = new Redemption(student.Id, product.Id, product.PointsCost, voucherCode)
            {
                PointsSpent = product.PointsCost
            };

            await _redemptionRepository.AddAsync(redemption);
            await _userRepository.UpdateAsync(student);
            await _productRepository.UpdateAsync(product);

            return new RedemptionResultDto
            {
                Success = true,
                Message = "Resgate realizado com sucesso!",
                RemainingPoints = student.TotalPoints,
                RedemptionId = redemption.Id,
                VoucherCode = redemption.VoucherCode,
                ExpiresAt = redemption.ExpiresAt
            };
        }
        catch (Exception ex)
        {
            return new RedemptionResultDto { Success = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<RedemptionDto>> GetStudentRedemptionsAsync(Guid studentId)
    {
        var redemptions = await _redemptionRepository.GetByStudentIdAsync(studentId);
        return redemptions.Select(RedemptionMapper.ToDto);
    }

    public async Task<IEnumerable<RedemptionDto>> GetAllAsync(string? status = null)
    {
        RedemptionStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<RedemptionStatus>(status, true, out var value))
                throw new ArgumentException("Status de resgate inválido.");
            parsedStatus = value;
        }

        var redemptions = await _redemptionRepository.GetAllAsync(parsedStatus);
        return redemptions.Select(RedemptionMapper.ToDto);
    }

    public async Task<RedemptionValidationResultDto> ValidateAsync(string voucherCode, Guid adminId)
    {
        var normalizedCode = voucherCode.Trim().ToUpperInvariant();
        var redemption = await _redemptionRepository.GetByVoucherCodeAsync(normalizedCode);
        if (redemption == null)
            return new RedemptionValidationResultDto { Success = false, Message = "Vale não encontrado." };

        try
        {
            redemption.MarkAsCollected(adminId);
            await _redemptionRepository.UpdateAsync(redemption);
            return new RedemptionValidationResultDto
            {
                Success = true,
                Message = "Vale validado. A recompensa pode ser entregue.",
                Redemption = RedemptionMapper.ToDto(redemption)
            };
        }
        catch (InvalidOperationException ex)
        {
            return new RedemptionValidationResultDto
            {
                Success = false,
                Message = ex.Message,
                Redemption = RedemptionMapper.ToDto(redemption)
            };
        }
    }

    private async Task<string> GenerateUniqueVoucherCodeAsync()
    {
        string code;
        do
        {
            var value = Convert.ToHexString(RandomNumberGenerator.GetBytes(4));
            code = $"PG-{value[..4]}-{value[4..]}";
        } while (await _redemptionRepository.GetByVoucherCodeAsync(code) != null);

        return code;
    }
}
