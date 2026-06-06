using Domain.Entities;
using IServices;

namespace ServicesTests.Stubs;

public class CrucesGeneratorServiceStub : ICrucesGeneratorService
{
    private List<Cruce> _generatedCruces;
    private List<Group> _lastGroups;
    private List<Stadium> _lastStadiums;
    private int _lastSeed;
    private List<Cruce> _lastRound;

    public CrucesGeneratorServiceStub() { }

    public List<Cruce> GenerateRoundOf32(Fixture fixture, List<Stadium> stadiums, int seed, int startMatchId)
    {
        _lastGroups = fixture.Groups.ToList();
        _lastStadiums = stadiums;
        _lastSeed = seed;
        return _generatedCruces ?? new List<Cruce>();
    }

    public List<Cruce> GenerateNextRound(List<Cruce> currentRound, List<Stadium> stadiums, int seed, int startMatchId)
    {
        _lastRound = currentRound;
        _lastStadiums = stadiums;
        _lastSeed = seed;
        return _generatedCruces ?? new List<Cruce>();
    }

    public void SetGeneratedCruces(List<Cruce> cruces)
    {
        _generatedCruces = cruces;
    }

    public List<Group> LastGroups => _lastGroups;
    public List<Stadium> LastStadiums => _lastStadiums;
    public int LastSeed => _lastSeed;
    public List<Cruce> LastRound => _lastRound;
}