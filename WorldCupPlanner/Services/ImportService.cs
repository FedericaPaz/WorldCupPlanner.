using Domain.Entities;
using Domain.Import;
using DTOs.Import;
using IRepository;
using IServices;
using IServices.TeamInterfaces;
using Services.Mappers;

namespace Services;

public class ImportService : IImportService
{
    private readonly ITeamRepository _teamRepository;
    private readonly TeamImporter _teamImporter;
    private readonly ITeamEditService _teamService;

    public ImportService(ITeamRepository teamRepository, ITeamEditService teamService, TeamImporter teamImporter)
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
        var validationErrors = ValidateBase64InCsv(csvContent);
        if (validationErrors.Any())
        {
            return new ImportResponse { Success = false, Errors = validationErrors };
        }
        
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

    private List<ImportErrorResponse> ValidateBase64InCsv(string csvContent)
    {
        var errors = new List<ImportErrorResponse>();
        var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) 
        {
            var columns = lines[i].Split(',');
            if (columns.Length < 4) continue;

            string flagData = columns[3].Trim(); 

            if (!string.IsNullOrWhiteSpace(flagData))
            {
                try
                {
                    Convert.FromBase64String(flagData.Replace("\r", "").Replace("\n", "").Trim());
                }
                catch
                {
                    errors.Add(new ImportErrorResponse { 
                        RowNumber = i + 1, 
                        Message = "El formato de la bandera en esta fila es inválido." 
                    });
                }
            }
        }
        return errors;
    }

    private void SaveImportedTeams(IReadOnlyList<Team> teams)
    {
        if (teams.Count != 48)
            throw new InvalidOperationException($"El CSV debe contener exactamente 48 equipos.");

        var porConfederacion = teams.GroupBy(t => t.Confederation.Name);
        foreach (var grupo in porConfederacion)
        {
            var conf = new Confederation(grupo.Key);
            if (grupo.Count() > conf.MaxTeams)
                throw new InvalidOperationException(
                    $"La confederación '{grupo.Key}' excede su límite de {conf.MaxTeams} equipos. Se encontraron {grupo.Count()}.");
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