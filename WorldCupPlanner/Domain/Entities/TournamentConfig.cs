namespace Domain.Entities;

public class TournamentConfig
{
    private const int DefaultSemillaFixture = 12345;
    private const int DefaultSemillaCompletar = 67890;
    private const int DefaultSemillaCrucesFase = 13579;
    private const int DefaultSemillaSimulation = 24680;
    private static readonly DateTime DefaultFechaInicioTorneo = new DateTime(2026, 6, 20);
    private const int DefaultMaxPartidosPorDia = 3;
    private const int DefaultSeparacionEntreFechas = 3;

    private int _fixtureSeed;
    private int _fillingSeed;
    private int _phaseDrawSeed;
    private int _simulationSeed;
    private DateTime _tournamentStartDate;
    private int _maxMatchesPerDay;
    private int _daysBetweenMatchdays;

    public int FixtureSeed
    {
        get { return _fixtureSeed; }
        set
        {
            EnsureNonNegative(value, nameof(FixtureSeed));
            _fixtureSeed = value;
        }
    }

    public int FillingSeed
    {
        get { return _fillingSeed; }
        set
        {
            EnsureNonNegative(value, nameof(FillingSeed));
            _fillingSeed = value;
        }
    }

    public int PhaseDrawSeed
    {
        get { return _phaseDrawSeed; }
        set
        {
            EnsureNonNegative(value, nameof(PhaseDrawSeed));
            _phaseDrawSeed = value;
        }
    }

    public int SimulationSeed
    {
        get { return _simulationSeed; }
        set
        {
            EnsureNonNegative(value, nameof(SimulationSeed));
            _simulationSeed = value;
        }
    }

    public DateTime TournamentStartDate
    {
        get { return _tournamentStartDate; }
        set { _tournamentStartDate = value; }
    }

    public int MaxMatchesPerDay
    {
        get { return _maxMatchesPerDay; }
        set
        {
            EnsurePositive(value, nameof(MaxMatchesPerDay));
            _maxMatchesPerDay = value;
        }
    }

    public int DaysBetweenMatchdays
    {
        get { return _daysBetweenMatchdays; }
        set
        {
            EnsureNonNegative(value, nameof(DaysBetweenMatchdays));
            _daysBetweenMatchdays = value;
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
        int fixtureSeed,
        int fillingSeed,
        int phaseDrawSeed,
        int simulationSeed,
        DateTime tournamentStartDate,
        int maxMatchesPerDay,
        int daysBetweenMatchdays)
    {
        FixtureSeed = fixtureSeed;
        FillingSeed = fillingSeed;
        PhaseDrawSeed = phaseDrawSeed;
        SimulationSeed = simulationSeed;
        TournamentStartDate = tournamentStartDate;
        MaxMatchesPerDay = maxMatchesPerDay;
        DaysBetweenMatchdays = daysBetweenMatchdays;
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