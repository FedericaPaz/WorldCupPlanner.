namespace DTOs.Config;

public class TournamentConfigDTO
{
    public int FixtureSeed { get; set; }
    public int FillingSeed { get; set; }
    public int PhaseDrawSeed { get; set; }
    public int SimulationSeed { get; set; }
    public DateTime TournamentStartDate { get; set; }
    public int MaxMatchesPerDay { get; set; }
    public int DaysBetweenMatchdays { get; set; }
}