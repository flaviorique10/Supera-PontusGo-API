namespace PontusGo.Application.DTOs;

public class CreateProductDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required int PointsCost { get; set; }
    public required int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}