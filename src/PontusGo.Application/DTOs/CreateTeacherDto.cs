namespace PontusGo.Application.DTOs;

public class CreateTeacherDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
