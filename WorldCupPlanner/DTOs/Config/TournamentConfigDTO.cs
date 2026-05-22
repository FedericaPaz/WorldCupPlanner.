namespace DTOs.Config;

public class TournamentConfigDTO
{
    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }
    public DateTime FechaInicioTorneo { get; set; }
    public int MaxPartidosPorDia { get; set; }
    public int SeparacionEntreFechas { get; set; }
}