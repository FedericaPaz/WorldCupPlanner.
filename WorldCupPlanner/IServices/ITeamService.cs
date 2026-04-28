using DTOs;

namespace IServices;

public interface ITeamService
{
    void Add(TeamDTOs.CreateTeamDTO request);
    TeamDTOs.TeamResponseDTO Get(string name);
    List<TeamDTOs.TeamResponseDTO> GetAll();
    void Delete(string name);
}