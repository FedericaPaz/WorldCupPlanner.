using Domain.Entities;
using DTOs.Fixture;
using IRepository;
using IServices;

namespace ServicesTests.Stubs;

public class FixtureServiceStub : IFixtureService
{
    private readonly IFixtureRepository _repository;

    public FixtureServiceStub(IFixtureRepository repository)
    {
        _repository = repository;
    }

    public Fixture GetFixtureEntity()
    {
        return _repository.GetFixture();
    }

    public FixtureResponse GetFixture()
    {
        if (!_repository.ContainsFixture())
            return new FixtureResponse();
        return new FixtureResponse();
    }

    public bool HasFixture() => _repository.ContainsFixture();

    public void GenerateFixture() { }

    public List<StandingResponse> GetStandingsForGroup(char groupLetter)
        => new List<StandingResponse>();
}