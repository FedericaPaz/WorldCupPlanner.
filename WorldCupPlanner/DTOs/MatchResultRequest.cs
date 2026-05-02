using System.ComponentModel.DataAnnotations;
namespace DTOs;

public class MatchResultRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "Los goles del local no pueden ser negativos")]
    public int homeGoals { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Los goles del visitante no pueden ser negativos")]
    public int awayGoals { get; set; }
}