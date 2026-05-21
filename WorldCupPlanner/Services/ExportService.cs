using Domain.Enums;
using IRepository;
using IServices;

namespace Services;

public class ExportService : IExportService
{
    private readonly IUserRepository _userRepository;

    public ExportService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public byte[] ExportFixtureToCsv(string userEmail)
    {
        ValidateCanExport(userEmail);

        return Array.Empty<byte>();
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
    
    public byte[] ExportFixtureToXlsx(string userEmail)
    {
        ValidateCanExport(userEmail);

        return Array.Empty<byte>();
    }
    
    public byte[] ExportLogsToCsv(string userEmail, DateTime from, DateTime to)
    {
        ValidateCanExport(userEmail);

        return Array.Empty<byte>();
    }
    
    public byte[] ExportLogsToXlsx(string userEmail, DateTime from, DateTime to)
    {
        ValidateCanExport(userEmail);

        return Array.Empty<byte>();
    }
}