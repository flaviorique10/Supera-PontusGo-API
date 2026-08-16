using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;

namespace PontusGo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetAllStudents()
    {
        return Ok(await _userService.GetAllStudentsAsync());
    }

    [HttpGet("students/{id:guid}")]
    public async Task<IActionResult> GetStudentProfile(Guid id)
    {
        var profile = await _userService.GetStudentProfileAsync(id);
        return profile == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(profile);
    }

    [HttpPost("students")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        try
        {
            var student = await _userService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetStudentProfile), new { id = student.Id }, student);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            return Ok(await _userService.CreateAsync(dto));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/add-points")]
    public async Task<IActionResult> AddPoints(Guid id, [FromQuery] int points, [FromQuery] string description)
    {
        if (points <= 0 || string.IsNullOrWhiteSpace(description))
            return BadRequest(new { message = "Informe uma quantidade positiva e uma descrição." });

        try
        {
            var result = await _userService.AddPointsAsync(id, points, description);
            return result == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
