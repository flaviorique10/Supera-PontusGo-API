namespace PontusGo.Application.DTOs;

public class CreateUserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    // 1 para Admin, 2 para Student
    public int RoleId { get; set; }
}