namespace IServices;

public interface IExportService
{
    byte[] ExportFixtureToCsv(string userEmail);
    byte[] ExportFixtureToXlsx(string userEmail);
}