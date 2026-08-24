using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;

namespace PontusGo.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IRedemptionRepository _redemptionRepository;

    public ProductService(IProductRepository productRepository, IRedemptionRepository redemptionRepository)
    {
        _productRepository = productRepository;
        _redemptionRepository = redemptionRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllActiveAsync()
    {
        var products = await _productRepository.GetAllActiveAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (dto.PointsCost <= 0)
            throw new ArgumentException("O custo em pontos deve ser maior que zero.");

        if (dto.StockQuantity < 0)
            throw new ArgumentException("O estoque não pode ser negativo.");

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            PointsCost = dto.PointsCost,
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim()
        };

        if (dto.StockQuantity > 0)
            product.AddStock(dto.StockQuantity);

        await _productRepository.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductDto?> UpdateStockAsync(Guid id, int newStockQuantity)
    {
        if (newStockQuantity < 0)
            throw new ArgumentException("A quantidade em estoque não pode ser negativa.");

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        product.SetStock(newStockQuantity);
        await _productRepository.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        product.Deactivate();
        await _productRepository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        product.Activate();
        await _productRepository.UpdateAsync(product);
        return true;
    }

    public async Task<(bool Success, string Message, bool DeletedPermanently)> DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return (false, "Produto não encontrado.", false);

        var hasRedemptions = await _redemptionRepository.HasRedemptionsForProductAsync(id);
        if (hasRedemptions)
        {
            product.Deactivate();
            await _productRepository.UpdateAsync(product);
            return (true, "Este item possui histórico de resgates e não pode ser apagado fisicamente. Ele foi desativado do catálogo.", false);
        }

        await _productRepository.DeleteAsync(product);
        return (true, "Recompensa excluída permanentemente do sistema.", true);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            PointsCost = product.PointsCost,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive
        };
    }
}
