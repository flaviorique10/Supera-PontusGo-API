using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontusGo.Application.DTOs;
using PontusGo.Application.Interfaces;
using PontusGo.Domain.Enums;

namespace PontusGo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("students")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> GetAllStudents()
    {
        return Ok(await _userService.GetAllStudentsAsync());
    }

    [HttpGet("students/{id:guid}")]
    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> GetStudentProfile(Guid id)
    {
        var profile = await _userService.GetStudentProfileAsync(id);
        return profile == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(profile);
    }

    [HttpGet("students/{id:guid}/daily-points-summary")]
    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> GetDailyPointsSummary(Guid id)
    {
        var summary = await _userService.GetDailyPointsSummaryAsync(id);
        return summary == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(summary);
    }

    [HttpPost("students")]
    [Authorize(Roles = "Admin,Teacher")]
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

    [HttpGet("teachers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTeachers()
    {
        return Ok(await _userService.GetAllTeachersAsync());
    }

    [HttpPost("teachers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto dto)
    {
        try
        {
            var teacher = await _userService.CreateTeacherAsync(dto);
            return Ok(teacher);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [HttpPost("{id:guid}/reset-password")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto)
    {
        try
        {
            var targetUser = await _userService.GetByIdAsync(id);
            if (targetUser == null)
                return NotFound(new { message = "Usuário não encontrado." });

            var callerRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                             ?? User.FindFirst("role")?.Value;

            // Professor só tem permissão para redefinir senha de Estudantes
            if (callerRole == UserRole.Teacher.ToString() && targetUser.Role != UserRole.Student.ToString())
            {
                return Forbid();
            }

            var success = await _userService.ResetPasswordAsync(id, dto.NewPassword);
            if (!success)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(new { message = "Senha redefinida com sucesso." });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:guid}/tuition-status")]
    [HttpPut("{id:guid}/tuition-status")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> UpdateTuitionStatus(Guid id, [FromBody] UpdateTuitionStatusDto dto)
    {
        try
        {
            var result = await _userService.UpdateTuitionStatusAsync(id, dto.Status);
            return result == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(result);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/award-daily-points")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> AwardDailyPoints(Guid id, [FromBody] AwardDailyPointsDto dto)
    {
        try
        {
            var result = await _userService.AwardDailyPointsAsync(id, dto);
            return result == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(result);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/add-points")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> AddPoints(Guid id, [FromQuery] int points, [FromQuery] string description)
    {
        if (points <= 0 || string.IsNullOrWhiteSpace(description))
            return BadRequest(new { message = "Informe uma quantidade positiva e uma descrição." });

        try
        {
            var result = await _userService.AddPointsAsync(id, points, description);
            return result == null ? NotFound(new { message = "Estudante não encontrado." }) : Ok(result);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
