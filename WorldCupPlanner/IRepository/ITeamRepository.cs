using Domain;

namespace IRepository;

public interface ITeamRepository
{
    ITeam GetByName(string name);
}