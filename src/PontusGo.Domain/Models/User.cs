using PontusGo.Domain.Enums;

namespace PontusGo.Domain.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        // O saldo de pontos só é alterado por métodos controlados
        public int TotalPoints { get; private set; }

        // Propriedades de navegação
        public ICollection<PointTransaction> Transactions { get; private set; } = new List<PointTransaction>();
        public ICollection<Redemption> Redemptions { get; private set; } = new List<Redemption>();

        public User()
        {
            Id = Guid.NewGuid();
        }

        public void AddPoints(int points)
        {
            if (points <= 0) throw new ArgumentException("Os pontos ganhos devem ser maiores que zero.");
            TotalPoints += points;
        }

        public void DeductPoints(int points)
        {
            if (points <= 0) throw new ArgumentException("A quantidade a deduzir deve ser maior que zero.");
            if (points > TotalPoints) throw new InvalidOperationException("Saldo de pontos insuficiente.");
            TotalPoints -= points;
        }
    }
}
