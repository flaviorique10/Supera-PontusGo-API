using PontusGo.Domain.Enums;

namespace PontusGo.Application.DTOs;

public class CreateStudentDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public TuitionStatus TuitionStatus { get; set; } = TuitionStatus.UpToDate;
}
