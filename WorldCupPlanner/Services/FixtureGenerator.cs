using Domain;
using IRepository;

namespace Services;

public class FixtureGenerator
{
    private readonly ITeamRepository _teamRepository;
    private readonly IStadiumRepository _stadiumRepository;
    
    public FixtureGenerator(ITeamRepository teamRepository, IStadiumRepository stadiumRepository)
    {
        _teamRepository = teamRepository;
        _stadiumRepository = stadiumRepository;
    }
    
    public Fixture GenerateGroupsStageFixture(TournamentConfig config)
    {
        var teams = _teamRepository.GetAll();
        var stadiums = _stadiumRepository.GetAllStadiums();
        if (teams.Count != 48)
            throw new Exception("Se necesitan 48 equipos");
        
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        return fixture;
    }
}