namespace IServices;

public interface IExportService
{
    byte[] ExportFixtureToCsv(string userEmail);
}