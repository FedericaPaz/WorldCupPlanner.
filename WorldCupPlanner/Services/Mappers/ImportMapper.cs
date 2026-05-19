using Domain.Import;
using DTOs.Import;

namespace Services.Mappers;

public static class ImportMapper
{
    public static ImportResponse ToResponse(ImportResult result)
    {
        return new ImportResponse
        {
            Success = result.Success,
            ImportedTeamsCount = result.Teams.Count,
            Errors = result.Errors
                .Select(error => ToResponse(error))
                .ToList()
        };
    }

    private static ImportErrorResponse ToResponse(ImportError error)
    {
        return new ImportErrorResponse
        {
            RowNumber = error.RowNumber,
            Message = error.Message
        };
    }
}