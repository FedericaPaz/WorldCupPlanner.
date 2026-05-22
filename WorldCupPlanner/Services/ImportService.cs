using Domain.Entities;
using Domain.Import;
using DTOs.Import;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class ImportService : IImportService
{
    private readonly ITeamRepository _teamRepository;
    private readonly TeamImporter _teamImporter;
    private readonly ITeamService _teamService;

    public ImportService(ITeamRepository teamRepository, ITeamService teamService, TeamImporter teamImporter)
    {
        if (teamRepository == null)
            throw new ArgumentNullException(nameof(teamRepository));
        if (teamService == null)
            throw new ArgumentNullException(nameof(teamService));
        if (teamImporter == null)
            throw new ArgumentNullException(nameof(teamImporter));

        _teamRepository = teamRepository;
        _teamService = teamService;
        _teamImporter = teamImporter;
    }

    public ImportResponse ImportTeams(string csvContent)
    {
        ImportResult result = _teamImporter.Import(csvContent);
        
        if (!result.Success) return ImportMapper.ToResponse(result);
        try
        {
            SaveImportedTeams(result.Teams);
        }
        catch (Exception ex)
        {
            return new ImportResponse
            {
                Success = false,
                Errors = new List<ImportErrorResponse>
                {
                    new ImportErrorResponse { RowNumber = 0, Message = ex.Message }
                }
            };
        }
        return ImportMapper.ToResponse(result);
    }

    private void SaveImportedTeams(IReadOnlyList<Team> teams)
    {
        if (teams.Count != 48)
            throw new InvalidOperationException($"El CSV debe contener exactamente 48 equipos. Se encontraron {teams.Count}.");

        _teamRepository.Clear();
        try
        {
            foreach (Team team in teams)
                _teamService.Add(TeamXMappers.FromEntity(team));
        }
        catch
        {
            _teamRepository.Clear();
            throw;
        }
    }
}