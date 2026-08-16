namespace PontusGo.Domain.Models
{
    public class Product
    {
        public Guid Id { get; private set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int PointsCost { get; set; }
        public int StockQuantity { get; private set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; private set; } = true;

        public Product()
        {
            Id = Guid.NewGuid();
        }

        public void DecreaseStock(int quantity = 1)
        {
            if (StockQuantity < quantity) throw new InvalidOperationException("Estoque insuficiente para este produto.");
            StockQuantity -= quantity;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("A quantidade a adicionar deve ser maior que zero.");
            StockQuantity += quantity;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
