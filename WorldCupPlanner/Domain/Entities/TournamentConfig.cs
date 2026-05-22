namespace Domain.Entities;

public class TournamentConfig
{
    private const int DefaultSemillaFixture = 12345;
    private const int DefaultSemillaCompletar = 67890;
    private const int DefaultSemillaCrucesFase = 13579;
    private const int DefaultSemillaSimulation = 24680;
    private static readonly DateTime DefaultFechaInicioTorneo = new DateTime(2026, 6, 1);
    private const int DefaultMaxPartidosPorDia = 3;
    private const int DefaultSeparacionEntreFechas = 3;

    private int _semillaFixture;
    private int _semillaCompletar;
    private int _semillaCrucesFase;
    private int _semillaSimulation;
    private DateTime _fechaInicioTorneo;
    private int _maxPartidosPorDia;
    private int _separacionEntreFechas;

    public int SemillaFixture
    {
        get { return _semillaFixture; }
        set
        {
            EnsureNonNegative(value, nameof(SemillaFixture));
            _semillaFixture = value;
        }
    }

    public int SemillaCompletar
    {
        get { return _semillaCompletar; }
        set
        {
            EnsureNonNegative(value, nameof(SemillaCompletar));
            _semillaCompletar = value;
        }
    }

    public int SemillaCrucesFase
    {
        get { return _semillaCrucesFase; }
        set
        {
            EnsureNonNegative(value, nameof(SemillaCrucesFase));
            _semillaCrucesFase = value;
        }
    }

    public int SemillaSimulation
    {
        get { return _semillaSimulation; }
        set
        {
            EnsureNonNegative(value, nameof(SemillaSimulation));
            _semillaSimulation = value;
        }
    }

    public DateTime FechaInicioTorneo
    {
        get { return _fechaInicioTorneo; }
        set { _fechaInicioTorneo = value; }
    }

    public int MaxPartidosPorDia
    {
        get { return _maxPartidosPorDia; }
        set
        {
            EnsurePositive(value, nameof(MaxPartidosPorDia));
            _maxPartidosPorDia = value;
        }
    }

    public int SeparacionEntreFechas
    {
        get { return _separacionEntreFechas; }
        set
        {
            EnsureNonNegative(value, nameof(SeparacionEntreFechas));
            _separacionEntreFechas = value;
        }
    }

    public TournamentConfig()
        : this(DefaultSemillaFixture,
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
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        EnsureNonNegative(seed, nameof(seed));

        Random random = new Random(seed);
        List<T> result = new List<T>(list);

        for (int i = result.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);
            (result[i], result[randomIndex]) = (result[randomIndex], result[i]);
        }

        return result;
    }

    private static void EnsureNonNegative(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }

    private static void EnsurePositive(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}