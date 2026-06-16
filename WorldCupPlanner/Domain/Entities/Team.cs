using Domain.Calculators;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Team : ITeam
{
    private const int MaxNameLength = 60;
    public const int MinRanking = 300;
    public const int MaxRanking = 2500;

    private string _name = string.Empty;
    private Confederation _confederation = null!;
    private int _fifaRanking;
    private int _currentRanking;
    private string _flagBase64 = string.Empty;
    private readonly IRankingCalculator _rankingCalculator;

    public string Name => _name;
    public Confederation Confederation => _confederation;
    public int FifaRanking => _fifaRanking;
    public int CurrentRanking => _currentRanking;

    public string FlagBase64
    {
        get => _flagBase64;
        private set => _flagBase64 = value;
    }

    private Team()
    {
        _rankingCalculator = new RankingCalculator();
    }

    public Team(string name, Confederation confederation, int fifaRanking, string flagBase64 = "")
        : this(name, confederation, fifaRanking, flagBase64, new RankingCalculator())
    {
    }

    // Constructor for dependency injection of the ranking calculator
    public Team(string name, Confederation confederation, int fifaRanking, string flagBase64, IRankingCalculator rankingCalculator)
    {
        ValidateName(name);
        ValidateConfederation(confederation);
        ValidateRanking(fifaRanking);

        _name = name;
        _confederation = confederation;
        _fifaRanking = fifaRanking;
        _currentRanking = fifaRanking;
        _flagBase64 = flagBase64;
        _rankingCalculator = rankingCalculator ?? new RankingCalculator();
    }

    public void UpdateFifaRanking(int newRanking)
    {
        ValidateRanking(newRanking);
        _fifaRanking = newRanking;
    }

    public void UpdateCurrentRanking(int newRanking)
    {
        ValidateRanking(newRanking);
        _currentRanking = newRanking;
    }

    public void UpdateConfederation(Confederation confederation)
    {
        ValidateConfederation(confederation);
        _confederation = confederation;
    }

    public void ResetRankingToFifa()
    {
        UpdateCurrentRanking(FifaRanking);
    }

    public void RecalcularRanking(int rankingRival, double resultado, TournamentPhase fase)
    {
        int nuevoRanking = _rankingCalculator.CalculateNewRanking(_currentRanking, rankingRival, resultado, fase);
        UpdateCurrentRanking(nuevoRanking);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del equipo no puede estar vacío.");
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"El nombre del equipo no puede superar los {MaxNameLength} caracteres.");
    }

    private void ValidateConfederation(Confederation confederation)
    {
        if (confederation is null)
            throw new ArgumentNullException(nameof(confederation), "La confederación del equipo no puede estar vacía.");
        if (string.IsNullOrWhiteSpace(confederation.Name))
            throw new ArgumentException("El nombre de la confederación no puede estar vacío.");
    }

    private void ValidateRanking(int ranking)
    {
        if (ranking < MinRanking || ranking > MaxRanking)
            throw new ArgumentException($"El ranking FIFA debe estar entre {MinRanking} y {MaxRanking}.");
    }
    
    public void UpdateFlag(string flagBase64)
    {
        _flagBase64 = flagBase64 ?? string.Empty;
    }

    public string GetConfederationName() => _confederation.Name;
}