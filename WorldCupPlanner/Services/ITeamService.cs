using Domain;
using DTOs;

namespace Services;

public interface ITeamService
{
    void Add(TeamDTOs.CreateTeamDTO request);
    TeamDTOs.TeamResponseDTO? Get(string name);
    List<TeamDTOs.TeamResponseDTO> GetAll();
    void Delete(string name);
}