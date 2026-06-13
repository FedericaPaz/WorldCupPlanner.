using Domain.Enums;
using IRepository;
using IServices;

namespace Services;

public class ExportService : IExportService
{
    private const string FixtureExportName = "Fixture";
    private const string LogsExportName = "Logs";
    private const string CsvFormat = "CSV";
    private const string XlsxFormat = "XLSX";

    private readonly IUserReadRepository _userRepository;
    private readonly ILogService _logService;

    public ExportService(IUserReadRepository userRepository, ILogService logService)
    {
        _userRepository = userRepository;
        _logService = logService;
    }

    public byte[] ExportFixtureToCsv(string userEmail)
    {
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, FixtureExportName, CsvFormat);

        return BuildEmptyExport(FixtureExportName, CsvFormat);
    }

    public byte[] ExportFixtureToXlsx(string userEmail)
    {
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, FixtureExportName, XlsxFormat);

        return BuildEmptyExport(FixtureExportName, XlsxFormat);
    }

    public byte[] ExportLogsToCsv(string userEmail, DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, LogsExportName, CsvFormat);

        return BuildEmptyExport(LogsExportName, CsvFormat);
    }

    public byte[] ExportLogsToXlsx(string userEmail, DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);
        ValidateCanExport(userEmail);
        RegisterExport(userEmail, LogsExportName, XlsxFormat);

        return BuildEmptyExport(LogsExportName, XlsxFormat);
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

    private static byte[] BuildEmptyExport(string exportName, string format)
    {
        return Array.Empty<byte>();
    }
}