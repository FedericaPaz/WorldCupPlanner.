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
        {
            foreach (Team team in result.Teams)
            {
                _teamRepository.Add(team);
            }
        }

        return ImportMapper.ToResponse(result);
    }
}