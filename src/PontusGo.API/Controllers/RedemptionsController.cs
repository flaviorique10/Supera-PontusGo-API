using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;

namespace PontusGo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RedemptionsController : ControllerBase
{
    private readonly IRedemptionService _redemptionService;

    public RedemptionsController(IRedemptionService redemptionService)
    {
        _redemptionService = redemptionService;
    }

    [HttpPost("{productId:guid}/redeem")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RedeemProduct(Guid productId)
    {
        if (!TryGetCurrentUserId(out var studentId)) return Unauthorized();

        var result = await _redemptionService.RedeemProductAsync(studentId, productId);
        return result.Success ? Ok(result) : BadRequest(new { message = result.Message });
    }

    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyRedemptions()
    {
        if (!TryGetCurrentUserId(out var studentId)) return Unauthorized();
        return Ok(await _redemptionService.GetStudentRedemptionsAsync(studentId));
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null)
    {
        try
        {
            return Ok(await _redemptionService.GetAllAsync(status));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("validate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Validate([FromBody] ValidateRedemptionDto dto)
    {
        if (!TryGetCurrentUserId(out var adminId)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(dto.VoucherCode))
            return BadRequest(new { message = "Informe o código do vale." });

        var result = await _redemptionService.ValidateAsync(dto.VoucherCode, adminId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out userId);
    }
}
