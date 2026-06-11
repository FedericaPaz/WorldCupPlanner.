using Domain.Entities;
using DTOs.Fixture;
using IRepository;
using IServices;

namespace ServicesTests.Stubs;

public class FixtureServiceStub : IFixtureService
{
    private readonly IFixtureRepository _repository;
    public string LastEngineUsed { get; private set; } = string.Empty;

    public FixtureServiceStub(IFixtureRepository repository)
    {
        _repository = repository;
    }

    public void GenerateFixture(string simulationEngineName = "Probabilístico")
    {
        LastEngineUsed = simulationEngineName;
    }

    public Fixture GetFixtureEntity()
    {
        return _repository.GetFixture();
    }

    public void DeleteFixture(Fixture fixture)
    { 
        _repository.RemoveFixture(fixture);
    }

    public FixtureResponse GetFixture()
    {
        if (!_repository.ContainsFixture())
            return new FixtureResponse();
        return new FixtureResponse();
    }

    public bool HasFixture() => _repository.ContainsFixture();

    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
        => new List<StandingResponse>();
}