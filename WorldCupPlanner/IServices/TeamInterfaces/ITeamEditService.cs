using DTOs.Team;

namespace IServices.TeamInterfaces;

public interface ITeamEditService
{
    void Add(TeamDTO request);
    void Update(string currentName, TeamDTO request);
    void Delete(string name);
    void GenerateMissingTeams();
}