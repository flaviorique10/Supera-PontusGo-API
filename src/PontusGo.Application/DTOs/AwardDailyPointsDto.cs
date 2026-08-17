namespace PontusGo.Application.DTOs;

public class AwardDailyPointsDto
{
    // Assiduidade = 10 pontos
    public bool Assiduidade { get; set; }

    // Participação = 10 pontos
    public bool Participacao { get; set; }

    // Fazer Tarefa = 10 pontos
    public bool FazerTarefa { get; set; }

    // Observação opcional ou personalizada
    public string? Observation { get; set; }
}
