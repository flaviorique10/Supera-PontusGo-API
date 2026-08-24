using PontusGo.Application.DTOs;

namespace PontusGo.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllActiveAsync();
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateStockAsync(Guid id, int newStockQuantity);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
    Task<(bool Success, string Message, bool DeletedPermanently)> DeleteAsync(Guid id);
}
