using DTOs;

namespace IServices;

public interface IImportService
{
    ImportResponse ImportTeams(string csvContent);
}