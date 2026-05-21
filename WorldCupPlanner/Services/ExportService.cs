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

    private readonly IUserRepository _userRepository;

    public ExportService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public byte[] ExportFixtureToCsv(string userEmail)
    {
        ValidateCanExport(userEmail);

        return BuildEmptyExport(FixtureExportName, CsvFormat);
    }

    public byte[] ExportFixtureToXlsx(string userEmail)
    {
        ValidateCanExport(userEmail);

        return BuildEmptyExport(FixtureExportName, XlsxFormat);
    }

    public byte[] ExportLogsToCsv(string userEmail, DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);
        ValidateCanExport(userEmail);

        return BuildEmptyExport(LogsExportName, CsvFormat);
    }

    public byte[] ExportLogsToXlsx(string userEmail, DateTime from, DateTime to)
    {
        ValidateDateRange(from, to);
        ValidateCanExport(userEmail);

        return BuildEmptyExport(LogsExportName, XlsxFormat);
    }

    private void ValidateCanExport(string userEmail)
    {
        var user = _userRepository.GetUser(userEmail);

        if (user is null)
        {
            throw new InvalidOperationException("El usuario no existe.");
        }

        var canExport =
            user.Roles.Contains(UserRole.SystemAdministrator) ||
            user.Roles.Contains(UserRole.Journalist);

        if (!canExport)
        {
            throw new UnauthorizedAccessException("El usuario no tiene permisos para exportar.");
        }
    }

    private static void ValidateDateRange(DateTime from, DateTime to)
    {
        if (from > to)
        {
            throw new ArgumentException("La fecha desde no puede ser posterior a la fecha hasta.");
        }
    }

    private static byte[] BuildEmptyExport(string exportName, string format)
    {
        return Array.Empty<byte>();
    }
}