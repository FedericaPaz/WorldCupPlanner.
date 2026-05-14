namespace Domain;

public class TournamentConfig
{
    private const int DefaultSemillaFixture = 12345;
    private const int DefaultSemillaCompletar = 67890;
    private const int DefaultSemillaCrucesFase = 13579;
    private const int DefaultSemillaSimulation = 24680;
    private static readonly DateTime DefaultFechaInicioTorneo = new DateTime(2026, 6, 1);
    private const int DefaultMaxPartidosPorDia = 3;
    private const int DefaultSeparacionEntreFechas = 3;

    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }
    public DateTime FechaInicioTorneo { get; set; }
    public int MaxPartidosPorDia { get; set; }
    public int SeparacionEntreFechas { get; set; }

    public TournamentConfig()
        : this(
            DefaultSemillaFixture,
            DefaultSemillaCompletar,
            DefaultSemillaCrucesFase,
            DefaultSemillaSimulation,
            DefaultFechaInicioTorneo,
            DefaultMaxPartidosPorDia,
            DefaultSeparacionEntreFechas)
    {
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

    public List<T> Shuffle<T>(List<T> list, int seed)
    {
        var random = new Random(seed);
        var result = new List<T>(list);
        for (int i = result.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (result[i], result[j]) = (result[j], result[i]);
        }
        return result;
    }
}