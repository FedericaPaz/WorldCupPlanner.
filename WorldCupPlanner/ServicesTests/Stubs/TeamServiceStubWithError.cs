using DTOs;
using IServices;

namespace ServicesTests.Stubs;

public class TeamServiceStubWithError : ITeamService
{
    public void Add(TeamDTO team)
    {
        throw new InvalidOperationException("Error al agregar equipo");
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