using DTOs.Import;

namespace IServices;

public interface IImportService
{
    ImportResponse ImportTeams(string csvContent);
}