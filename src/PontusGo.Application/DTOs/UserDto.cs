namespace PontusGo.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
}