using PontusGo.Domain.Enums;

namespace PontusGo.Domain.Models;

public class Redemption
{
    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid ProductId { get; private set; }
    public required int PointsSpent { get; set; }
    public DateTime CreatedAt { get; private set; }
    public string VoucherCode { get; private set; } = string.Empty;
    public RedemptionStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? CollectedAt { get; private set; }
    public Guid? ValidatedByAdminId { get; private set; }

    public User? Student { get; private set; }
    public Product? Product { get; private set; }

    public Redemption()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddDays(7);
        Status = RedemptionStatus.Pending;
    }

    public Redemption(Guid studentId, Guid productId, int pointsSpent, string voucherCode) : this()
    {
        StudentId = studentId;
        ProductId = productId;
        PointsSpent = pointsSpent;
        VoucherCode = voucherCode;
    }

    public void MarkAsCollected(Guid adminId)
    {
        if (Status == RedemptionStatus.Collected)
            throw new InvalidOperationException("Este vale já foi utilizado.");

        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationException("Este vale está expirado.");

        Status = RedemptionStatus.Collected;
        CollectedAt = DateTime.UtcNow;
        ValidatedByAdminId = adminId;
    }
}
