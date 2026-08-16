using PontusGo.Application.DTOs;

namespace PontusGo.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto);
}