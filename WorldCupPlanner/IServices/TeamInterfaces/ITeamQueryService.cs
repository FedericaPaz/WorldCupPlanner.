using DTOs.Team;

namespace IServices.TeamInterfaces;

public interface ITeamQueryService
{
    TeamDTO? Get(string name);
    List<TeamDTO> GetAll();
    List<ConfederationDTO> GetAllConfederations();
}