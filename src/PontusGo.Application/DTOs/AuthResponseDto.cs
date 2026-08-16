namespace PontusGo.Application.DTOs;

public class AuthResponseDto
{
    public required UserDto User { get; set; }
    public required string Token { get; set; }
}