using Domain;
using DTOs;

namespace IServices;

public interface IImportService
{
    ImportResult ImportTeams(string csvContent);
}