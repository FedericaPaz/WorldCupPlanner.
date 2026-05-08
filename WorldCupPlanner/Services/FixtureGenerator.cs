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
        
        var orderedStadiums = stadiums.OrderBy(s => s.StadiumName).ToList();
        var clock = new ClockStub(DateTime.Now);
        
        foreach (var group in fixture.GetGroups)
        {
            var groupTeams = group.TeamsInGroup.ToList();
        
            var locales = new int[] { 0, 1, 0, 1, 0, 2 };
            var visitantes = new int[] { 3, 2, 2, 3, 1, 3 };
        
            for (int i = 0; i < 6; i++)
            {
                int jornada = i / 2;
                DateTime fecha = config.FechaInicioTorneo.AddDays(jornada * config.SeparacionEntreFechas);
                fecha = (i % 2 == 0) ? fecha.Date.AddHours(14) : fecha.Date.AddHours(18);
                var estadio = orderedStadiums[i % orderedStadiums.Count];
                var match = new Match(fecha, estadio,groupTeams[locales[i]], groupTeams[visitantes[i]], group.GroupLetter, clock);
                group.AddMatchToGroup(match);
            }
        }
        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        return fixture;
    }
}