using Domain;

namespace IRepository;

public interface ITeamRepository
{
    void Add(Team team);
    Team? Get(string name);
    void Update(Team team);
    void Delete(Team team);
    List<Team> GetAll();
}