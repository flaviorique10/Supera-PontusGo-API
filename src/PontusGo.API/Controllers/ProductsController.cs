using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;

namespace PontusGo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActive()
    {
        return Ok(await _productService.GetAllActiveAsync());
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> GetAllForAdmin()
    {
        return Ok(await _productService.GetAllAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAllActive), new { id = product.Id }, product);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}/stock")]
    [HttpPut("{id:guid}/stock")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto dto)
    {
        try
        {
            var product = await _productService.UpdateStockAsync(id, dto.StockQuantity);
            return product == null ? NotFound(new { message = "Produto não encontrado." }) : Ok(product);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        return await _productService.DeactivateAsync(id) ? Ok(new { message = "Recompensa desativada do catálogo com sucesso." }) : NotFound();
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Activate(Guid id)
    {
        return await _productService.ActivateAsync(id) ? Ok(new { message = "Recompensa reativada no catálogo com sucesso." }) : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result.Success) return NotFound(new { message = result.Message });
        return Ok(new { message = result.Message, deletedPermanently = result.DeletedPermanently });
    }
}
