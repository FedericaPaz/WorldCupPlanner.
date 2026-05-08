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
        
        if (stadiums.Count < 4)
            throw new Exception("Se necesitan 4 estadios");
        
        var fixture = new Fixture(config);
        fixture.InitializeGroups();
        
        var orderedTeams = teams.OrderBy(t => t.FifaRanking).ToList();
        for (int i = 0; i < orderedTeams.Count; i++)
        {
            int groupIndex = i % 12;
            fixture.GetGroups[groupIndex].AddTeamToGroup(orderedTeams[i]);
        }
        
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        return fixture;
    }
}