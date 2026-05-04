using DTOs;

namespace IServices;

public interface ITeamService
{
    void Add(TeamDTO request);
    TeamDTO? Get(string name);
    List<TeamDTO> GetAll();
    void Delete(string name);
    List<ConfederationDTO> GetAllConfederations();
}