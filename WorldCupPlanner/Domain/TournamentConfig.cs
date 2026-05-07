namespace Domain;

public class TournamentConfig
{
    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }
    
    public DateTime FechaInicioTorneo { get; set; } = new DateTime(2026, 6, 1);
    public int MaxPartidosPorDia { get; set; } = 3;
    public int SeparacionEntreFechas { get; set; } = 3;
    
    public TournamentConfig()
    {
        SemillaFixture = 12345;
        SemillaCompletar = 67890;
        SemillaCrucesFase = 13579;
        SemillaSimulation = 24680;
        FechaInicioTorneo = new DateTime(2026, 6, 1);
        MaxPartidosPorDia = 3;
        SeparacionEntreFechas = 3;
    }

    public TournamentConfig(
        int semillaFixture, 
        int semillaCompletar, 
        int semillaCrucesFase, 
        int semillaSimulation,
        DateTime fechaInicioTorneo, 
        int maxPartidosPorDia, 
        int separacionEntreFechas)
    {
        if (semillaFixture < 0)
            throw new ArgumentOutOfRangeException(nameof(semillaFixture));
        
        if (semillaCompletar < 0)
            throw new ArgumentOutOfRangeException(nameof(semillaCompletar));
        
        if (semillaCrucesFase < 0)
            throw new ArgumentOutOfRangeException(nameof(semillaCrucesFase));
        
        if (semillaSimulation < 0)
            throw new ArgumentOutOfRangeException(nameof(semillaSimulation));
        
        if (maxPartidosPorDia <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxPartidosPorDia));
        
        if (separacionEntreFechas < 0)
            throw new ArgumentOutOfRangeException(nameof(separacionEntreFechas));
        
        SemillaFixture = semillaFixture;
        SemillaCompletar = semillaCompletar;
        SemillaCrucesFase = semillaCrucesFase;
        SemillaSimulation = semillaSimulation;
        FechaInicioTorneo = fechaInicioTorneo;
        MaxPartidosPorDia = maxPartidosPorDia;
        SeparacionEntreFechas = separacionEntreFechas;
    }
}