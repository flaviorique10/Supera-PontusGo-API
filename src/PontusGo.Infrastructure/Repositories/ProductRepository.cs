using Microsoft.EntityFrameworkCore;
using PontusGo.Domain.Interfaces;
using PontusGo.Domain.Models;
using PontusGo.Infrastructure.Data;

namespace PontusGo.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly PontusGoDbContext _context;

    public ProductRepository(PontusGoDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.PointsCost)
            .ThenBy(product => product.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .OrderByDescending(product => product.IsActive)
            .ThenBy(product => product.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
