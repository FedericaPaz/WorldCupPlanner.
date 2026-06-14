namespace IServices;

public interface IExportService
{
    string ExportFixtureToCsv(string userEmail);
    string ExportFixtureToXlsx(string userEmail);
    string ExportLogsToCsv(string userEmail, DateTime from, DateTime to);
    string ExportLogsToXlsx(string userEmail, DateTime from, DateTime to);
}