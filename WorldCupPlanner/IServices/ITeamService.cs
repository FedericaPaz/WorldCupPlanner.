using DTOs;

namespace IServices;

public interface ITeamService
{
    void Add(TeamDTO request);
    TeamDTO? Get(string name);
    List<TeamDTO> GetAll();
    void Update(string currentName, TeamDTO request);
    void Delete(string name);
    List<ConfederationDTO> GetAllConfederations();
    void GenerateMissingTeams();
}