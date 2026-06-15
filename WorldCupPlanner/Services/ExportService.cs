using System.Text;
using ClosedXML.Excel;
using Domain.Enums;
using IRepository;
using IServices;
using IServices.UserInterfaces;

namespace Services;

public class ExportService : IExportService
{
    private const string FixtureExportName = "Fixture";
    private const string LogsExportName = "Logs";
    private const string CsvFormat = "CSV";
    private const string XlsxFormat = "XLSX";

    private readonly IUserQueryService _userQueryService;
    private readonly ILogService _logService;
    private readonly IFixtureRepository _fixtureRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly ICrucesRepository _crucesRepository;

    public ExportService(IUserQueryService userQueryService, ILogService logService, IFixtureRepository fixtureRepository, IMatchRepository matchRepository, ICrucesRepository crucesRepository)
    {
        if (userQueryService is null) throw new ArgumentNullException(nameof(userQueryService));
        if (fixtureRepository is null) throw new ArgumentNullException(nameof(fixtureRepository));
        if (matchRepository is null) throw new ArgumentNullException(nameof(matchRepository));
        if (crucesRepository is null) throw new ArgumentNullException(nameof(crucesRepository));

        _userQueryService = userQueryService;
        _log_service_check(logService);
        _logService = logService;
        _fixtureRepository = fixtureRepository;
        _matchRepository = matchRepository;
        _crucesRepository = crucesRepository;
    }

    public string ExportFixtureToCsv(string userEmail)
    {
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, FixtureExportName, CsvFormat);

        var csv = BuildFixtureCsv();
        return SaveTextFile(FixtureExportName, "csv", csv);
    }

    public string ExportFixtureToXlsx(string userEmail)
    {
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, FixtureExportName, XlsxFormat);

        return SaveWorkbook(FixtureExportName, "xlsx", ws =>
        {
            // Header
            ws.Cell(1, 1).Value = "Fase";
            ws.Cell(1, 2).Value = "Grupo";
            ws.Cell(1, 3).Value = "Fecha";
            ws.Cell(1, 4).Value = "Estadio";
            ws.Cell(1, 5).Value = "Equipo local";
            ws.Cell(1, 6).Value = "Goles local";
            ws.Cell(1, 7).Value = "Equipo visitante";
            ws.Cell(1, 8).Value = "Goles visitante";
            ws.Cell(1, 9).Value = "Estado";

            var fixture = _fixtureRepository.GetFixture();
            var matches = _matchRepository.GetAllMatches();
            fixture.CargarPartidos(matches);

            var row = 2;
            foreach (var group in fixture.Groups)
            {
                var orderedMatches = group.MatchesInGroup.OrderBy(m => m.DateMatch);
                foreach (var match in orderedMatches)
                {
                    ws.Cell(row, 1).Value = "Grupos";
                    ws.Cell(row, 2).Value = group.GroupLetter.ToString();
                    ws.Cell(row, 3).Value = match.DateMatch;
                    ws.Cell(row, 4).Value = match.Stadium?.StadiumName ?? string.Empty;
                    ws.Cell(row, 5).Value = match.HomeTeam?.Name ?? string.Empty;
                    ws.Cell(row, 6).Value = match.HomeGoals.HasValue ? match.HomeGoals.Value.ToString() : string.Empty;
                    ws.Cell(row, 7).Value = match.AwayTeam?.Name ?? string.Empty;
                    ws.Cell(row, 8).Value = match.AwayGoals.HasValue ? match.AwayGoals.Value.ToString() : string.Empty;
                    ws.Cell(row, 9).Value = (match.HomeGoals.HasValue && match.AwayGoals.HasValue) ? "Finalizado" : "Pendiente";
                    row++;
                }
            }

            // Append cruces/eliminatorias if exist
            if (_crucesRepository != null && _crucesRepository.HasCruces())
            {
                var cruces = _crucesRepository.GetAllCruces();
                foreach (var cruce in cruces)
                {
                    var match = cruce.Match;
                    ws.Cell(row, 1).Value = cruce.Name;
                    ws.Cell(row, 2).Value = string.Empty;
                    ws.Cell(row, 3).Value = match.DateMatch;
                    ws.Cell(row, 4).Value = match.Stadium?.StadiumName ?? string.Empty;
                    ws.Cell(row, 5).Value = match.HomeTeam?.Name ?? string.Empty;
                    ws.Cell(row, 6).Value = match.HomeGoals.HasValue ? match.HomeGoals.Value.ToString() : string.Empty;
                    ws.Cell(row, 7).Value = match.AwayTeam?.Name ?? string.Empty;
                    ws.Cell(row, 8).Value = match.AwayGoals.HasValue ? match.AwayGoals.Value.ToString() : string.Empty;
                    ws.Cell(row, 9).Value = (match.HomeGoals.HasValue && match.AwayGoals.HasValue) ? "Finalizado" : "Pendiente";
                    row++;
                }
            }

            ws.Columns().AdjustToContents();
        });
    }

    public string ExportLogsToCsv(string userEmail, DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, LogsExportName, CsvFormat);

        var logs = _logService.GetAll()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,UserEmail,Category,Action,Detail");

        foreach (var log in logs)
        {
            var timestamp = log.Timestamp.ToString("o");
            var user = EscapeCsv(log.UserEmail);
            var category = EscapeCsv(log.Category);
            var action = EscapeCsv(log.Action);
            var detail = EscapeCsv(log.Detail);
            sb.AppendLine($"{timestamp},{user},{category},{action},{detail}");
        }

        return SaveTextFile(LogsExportName, "csv", sb.ToString());
    }

    public string ExportLogsToXlsx(string userEmail, DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, LogsExportName, XlsxFormat);

        var logs = _logService.GetAll()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to)
            .ToList();

        return SaveWorkbook(LogsExportName, "xlsx", ws =>
        {
            // Header
            ws.Cell(1, 1).Value = "Timestamp";
            ws.Cell(1, 2).Value = "UserEmail";
            ws.Cell(1, 3).Value = "Category";
            ws.Cell(1, 4).Value = "Action";
            ws.Cell(1, 5).Value = "Detail";

            var row = 2;
            foreach (var log in logs)
            {
                ws.Cell(row, 1).Value = log.Timestamp;
                ws.Cell(row, 2).Value = log.UserEmail;
                ws.Cell(row, 3).Value = log.Category;
                ws.Cell(row, 4).Value = log.Action;
                ws.Cell(row, 5).Value = log.Detail;
                row++;
            }

            ws.Columns().AdjustToContents();
        });
    }

    private void _log_service_check(ILogService logService)
    {
        if (logService is null) throw new ArgumentNullException(nameof(logService));
    }

    private void ValidateCanExport(string userEmail)
    {
        if (!_userQueryService.CanExport(userEmail))
            throw new UnauthorizedAccessException("El usuario no tiene permisos para exportar.");
    }

    private static void ValidateDateRange(DateTime from, DateTime to)
    {
        if (from > to)
        {
            throw new ArgumentException("La fecha desde no puede ser posterior a la fecha hasta.");
        }
    }

    private void RegisterExport(string userEmail, string exportName, string format)
    {
        _logService.Register(userEmail, "Export", "FileExported", $"El usuario {userEmail} exportó {exportName} en formato {format}.");
    }

    private string BuildFixtureCsv()
    {
        var fixture = _fixtureRepository.GetFixture();
        var matches = _matchRepository.GetAllMatches();
        fixture.CargarPartidos(matches);

        var sb = new StringBuilder();
        sb.AppendLine("Fase,Grupo,Fecha,Estadio,Equipo local,Goles local,Equipo visitante,Goles visitante,Estado");

        foreach (var group in fixture.Groups)
        {
            var orderedMatches = group.MatchesInGroup.OrderBy(m => m.DateMatch);
            foreach (var match in orderedMatches)
            {
                var fase = "Grupos";
                var grupo = group.GroupLetter.ToString();
                var fecha = match.DateMatch.ToString("yyyy-MM-dd HH:mm");
                var estadio = EscapeCsv(match.Stadium?.StadiumName ?? string.Empty);
                var home = EscapeCsv(match.HomeTeam?.Name ?? string.Empty);
                var away = EscapeCsv(match.AwayTeam?.Name ?? string.Empty);
                var homeGoals = match.HomeGoals.HasValue ? match.HomeGoals.Value.ToString() : string.Empty;
                var awayGoals = match.AwayGoals.HasValue ? match.AwayGoals.Value.ToString() : string.Empty;
                var estado = (match.HomeGoals.HasValue && match.AwayGoals.HasValue) ? "Finalizado" : "Pendiente";

                sb.AppendLine($"{EscapeCsv(fase)},{grupo},{fecha},{estadio},{home},{homeGoals},{away},{awayGoals},{estado}");
            }
        }

        if (_crucesRepository != null && _crucesRepository.HasCruces())
        {
            var cruces = _crucesRepository.GetAllCruces();
            foreach (var cruce in cruces)
            {
                var match = cruce.Match;
                var fase = cruce.Name;
                var grupo = string.Empty;
                var fecha = match.DateMatch.ToString("yyyy-MM-dd HH:mm");
                var estadio = EscapeCsv(match.Stadium?.StadiumName ?? string.Empty);
                var home = EscapeCsv(match.HomeTeam?.Name ?? string.Empty);
                var away = EscapeCsv(match.AwayTeam?.Name ?? string.Empty);
                var homeGoals = match.HomeGoals.HasValue ? match.HomeGoals.Value.ToString() : string.Empty;
                var awayGoals = match.AwayGoals.HasValue ? match.AwayGoals.Value.ToString() : string.Empty;
                var estado = (match.HomeGoals.HasValue && match.AwayGoals.HasValue) ? "Finalizado" : "Pendiente";

                sb.AppendLine($"{EscapeCsv(fase)},{grupo},{fecha},{estadio},{home},{homeGoals},{away},{awayGoals},{estado}");
            }
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string input)
    {
        if (input is null) return string.Empty;
        if (input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r'))
        {
            return '"' + input.Replace("\"", "\"\"") + '"';
        }
        return input;
    }

    private string SaveTextFile(string baseName, string ext, string content)
    {
        var exportsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
        Directory.CreateDirectory(exportsPath);
        var filename = $"{baseName.ToLowerInvariant()}_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
        var filePath = Path.Combine(exportsPath, filename);
        File.WriteAllText(filePath, content, Encoding.UTF8);
        return $"/exports/{filename}";
    }

    private string SaveWorkbook(string baseName, string ext, Action<IXLWorksheet> fill)
    {
        var exportsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
        Directory.CreateDirectory(exportsPath);
        var filename = $"{baseName.ToLowerInvariant()}_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
        var filePath = Path.Combine(exportsPath, filename);

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add("Sheet1");
            fill(ws);
            wb.SaveAs(filePath);
        }

        return $"/exports/{filename}";
    }
}