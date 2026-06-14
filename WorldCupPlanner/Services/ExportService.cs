using System.Text;
using ClosedXML.Excel;
using Domain.Enums;
using IRepository;
using IServices;
using DTOs.Log;

namespace Services;

public class ExportService : IExportService
{
    private const string FixtureExportName = "Fixture";
    private const string LogsExportName = "Logs";
    private const string CsvFormat = "CSV";
    private const string XlsxFormat = "XLSX";

    private readonly IUserRepository _userRepository;
    private readonly ILogService _logService;

    public ExportService(IUserRepository userRepository, ILogService logService)
    {
        if (userRepository is null) throw new ArgumentNullException(nameof(userRepository));
        _userRepository = userRepository;
        _log_service_check(logService);
        _logService = logService;
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
            // Minimal fixture content: header + generated timestamp
            ws.Cell(1, 1).Value = "GeneratedAt";
            ws.Cell(1, 2).Value = "Description";
            ws.Cell(2, 1).Value = DateTime.Now.ToString("o");
            ws.Cell(2, 2).Value = "Fixture export";
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
        var user = _userRepository.GetUser(userEmail)
                   ?? throw new InvalidOperationException("El usuario no existe.");

        if (!user.CanExport())
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
        var sb = new StringBuilder();
        sb.AppendLine("GeneratedAt,Description");
        sb.AppendLine($"{DateTime.Now:o},\"Fixture export\"");
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