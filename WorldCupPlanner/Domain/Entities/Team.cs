using Domain.Interfaces;

namespace Domain.Entities;

public class Team : ITeam
{
    private const int MaxNameLength = 60;
    public const int MinRanking = 300;
    public const int MaxRanking = 2500;

    private string _name;
    private Confederation _confederation;
    private int _fifaRanking;
    private int _currentRanking;
    
    public string Name => _name;

    public Confederation Confederation => _confederation;

    public int FifaRanking => _fifaRanking;
    
    public int CurrentRanking => _currentRanking;

    private Team()
    {
        
    }
    
    public Team(string name, Confederation confederation, int fifaRanking)
    {
        ValidateName(name);
        ValidateConfederation(confederation);
        ValidateRanking(fifaRanking);

        _name = name;
        _confederation = confederation;
        _fifaRanking = fifaRanking;
        _currentRanking = fifaRanking;
    }

    public void UpdateFifaRanking(int newRanking)
    {
        ValidateRanking(newRanking);
        _fifaRanking = newRanking;
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

    public void UpdateCurrentRanking(int newRanking)
    {
        ValidateRanking(newRanking);
        _currentRanking = newRanking;
    }
    
}