using Domain.Entities;

namespace IRepository;

public interface ITeamRepository
{
    void Add(Team team);
    Team? Get(string name);
    void Update(Team team);
    void Delete(Team team);
    List<Team> GetAll();
    void Clear();
    void Rename(string oldName, Team newTeam);
}