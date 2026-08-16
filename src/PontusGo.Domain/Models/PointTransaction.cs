namespace PontusGo.Domain.Models
{
    public class PointTransaction
    {
        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public required int PointsAwarded { get; set; }
        public required string ActivityDescription { get; set; }
        public DateTime CreatedAt { get; private set; }

        public User? Student { get; private set; }

        public PointTransaction()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public PointTransaction(Guid studentId, int points, string description) : this()
        {
            StudentId = studentId;
            PointsAwarded = points;
            ActivityDescription = description;
        }
    }
}
