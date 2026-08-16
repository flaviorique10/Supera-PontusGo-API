using PontusGo.Application.DTOs;
using PontusGo.Domain.Models;

namespace PontusGo.Application.Services;

internal static class RedemptionMapper
{
    public static RedemptionDto ToDto(Redemption redemption)
    {
        return new RedemptionDto
        {
            Id = redemption.Id,
            StudentId = redemption.StudentId,
            StudentName = redemption.Student?.Name ?? string.Empty,
            ProductId = redemption.ProductId,
            ProductName = redemption.Product?.Name ?? string.Empty,
            ProductImageUrl = redemption.Product?.ImageUrl,
            PointsSpent = redemption.PointsSpent,
            VoucherCode = redemption.VoucherCode,
            Status = redemption.Status.ToString(),
            CreatedAt = redemption.CreatedAt,
            ExpiresAt = redemption.ExpiresAt,
            CollectedAt = redemption.CollectedAt,
            ValidatedByAdminId = redemption.ValidatedByAdminId
        };
    }
}
