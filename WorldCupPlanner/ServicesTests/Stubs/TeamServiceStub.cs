using IServices;
using IRepository;
using Domain.Entities;
using DTOs.Team;

namespace ServicesTests.Stubs;

public class TeamServiceStub(ITeamRepository teamRepository) : ITeamService
{
    public void Add(TeamDTO request)
    {
        var confederation = new Confederation(request.ConfederationName);
        var team = new Team(request.Name, confederation, request.FifaRanking);
        teamRepository.Add(team);
    }

    public TeamDTO? Get(string name)
    {
        throw new NotImplementedException();
    }

    public List<TeamDTO> GetAll()
    {
        throw new NotImplementedException();
    }

    public void Update(string currentName, TeamDTO request)
    {
        throw new NotImplementedException();
    }

    public void Delete(string name)
    {
        throw new NotImplementedException();
    }

    public List<ConfederationDTO> GetAllConfederations()
    {
        throw new NotImplementedException();
    }

    public void GenerateMissingTeams()
    {
        throw new NotImplementedException();
    }
}