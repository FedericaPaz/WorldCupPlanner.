using Domain.Entities;
using DTOs.Team;
using IRepository;
using IServices;
using IServices.TeamInterfaces;

namespace ServicesTests.Stubs;

public class TeamRepositoryStubWithError : ITeamRepository
{
    public void Add(Team team)
    {
        throw new InvalidOperationException("Error al agregar equipo");
    }

    public Team? Get(string name)
    {
        return null;
    }

    public void Update(Team team)
    {
        throw new NotImplementedException();
    }

    public void Delete(Team team)
    {
        throw new NotImplementedException();
    }

    public List<Team> GetAll()
    {
        return new List<Team>();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }
}
