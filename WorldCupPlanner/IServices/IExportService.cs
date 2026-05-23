namespace IServices;

public interface IExportService
{
    byte[] ExportFixtureToCsv(string userEmail);
    byte[] ExportFixtureToXlsx(string userEmail);
    byte[] ExportLogsToCsv(string userEmail, DateTime from, DateTime to);
    byte[] ExportLogsToXlsx(string userEmail, DateTime from, DateTime to);
}