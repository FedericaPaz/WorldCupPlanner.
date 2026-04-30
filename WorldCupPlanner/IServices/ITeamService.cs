using DTOs;

namespace IServices;

public interface ITeamService
{
    void Add(TeamDTOs.TeamDTO request);
    TeamDTOs.TeamDTO? Get(string name);
    List<TeamDTOs.TeamDTO> GetAll();
    void Delete(string name);
}