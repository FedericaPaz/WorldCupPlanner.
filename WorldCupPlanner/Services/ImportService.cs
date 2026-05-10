using Domain;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class ImportService : IImportService
{
    private readonly ITeamRepository _teamRepository;
    private readonly TeamImporter _teamImporter;

    public ImportService(ITeamRepository teamRepository)
    {
        if (teamRepository == null)
            throw new ArgumentNullException(nameof(teamRepository));

        _teamRepository = teamRepository;
        _teamImporter = new TeamImporter();
    }

    public ImportResponse ImportTeams(string csvContent)
    {
        ImportResult result = _teamImporter.Import(csvContent);

        if (result.Success)
            SaveImportedTeams(result.Teams);

        return ImportMapper.ToResponse(result);
    }

    private void SaveImportedTeams(IReadOnlyList<Team> teams)
    {
        foreach (Team team in teams)
        {
            _teamRepository.Add(team);
        }
    }
}