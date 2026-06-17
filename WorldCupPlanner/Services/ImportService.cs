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

    public ImportService(ITeamRepository teamRepository, TeamImporter teamImporter)
    {
        if (teamRepository == null)
            throw new ArgumentNullException(nameof(teamRepository));
        if (teamImporter == null)
            throw new ArgumentNullException(nameof(teamImporter));

        _teamRepository = teamRepository;
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
            return new ImportResponse { Success = false, Errors = new List<ImportErrorResponse> { new ImportErrorResponse { RowNumber = 0, Message = ex.Message } } };
        }
        return ImportMapper.ToResponse(result);
    }

    private void SaveImportedTeams(IReadOnlyList<Team> teams)
    {
        if (teams.Count != 48)
            throw new InvalidOperationException($"El CSV debe contener exactamente 48 equipos.");

        var byConfederation = teams.GroupBy(t => t.Confederation.Name);
        foreach (var group in byConfederation)
        {
            var conf = new Confederation(group.Key);
            if (group.Count() > conf.MaxTeams)
                throw new InvalidOperationException(
                    $"La confederación '{group.Key}' excede su límite de {conf.MaxTeams} equipos. Se encontraron {group.Count()}.");
        }

        _teamRepository.Clear();
        try
        {
            foreach (Team team in teams)
                _teamRepository.Add(team);
        }
        catch
        {
            _teamRepository.Clear();
            throw;
        }
    }
}