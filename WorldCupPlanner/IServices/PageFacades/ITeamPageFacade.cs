using DTOs.Team;

namespace IServices.PageFacades;

public interface ITeamPageFacade
{
    List<TeamDTO> GetAll();
    TeamDTO? Get(string name);
    List<ConfederationDTO> GetAllConfederations();
    void Add(TeamDTO dto, string userEmail);
    void Update(string currentName, TeamDTO dto, string userEmail);
    void Delete(string name, string userEmail);
    void GenerateMissingTeams(string userEmail);
}
