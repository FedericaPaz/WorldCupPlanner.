using Domain.Entities;
using Domain.Import.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain.Import;

public class TeamImportValidator : ITeamImportValidator
{
    private const string NameHeader = "Nombre";
    private const string ConfederationHeader = "Confederación";
    private const string RankingHeader = "RankingFIFA";
    private const string FlagHeader = "Bandera"; 
    public List<ImportError> ValidateHeaders(IReadOnlyList<string> headers)
    {
        List<ImportError> errors = new List<ImportError>();
        
        if (!headers.Contains(NameHeader) || 
            !headers.Contains(ConfederationHeader) || 
            !headers.Contains(RankingHeader) || 
            !headers.Contains(FlagHeader))
        {
            errors.Add(new ImportError(1, "El encabezado del CSV no es válido. Debe contener Nombre, Confederación, RankingFIFA y Bandera."));
        }
        return errors;
    }
    
    public List<ImportError> ValidateRow(ParsedCsvRow row, HashSet<string> importedNames)
    {
        List<ImportError> errors = new List<ImportError>();

        string name = row.GetValue(NameHeader);
        string confederationName = row.GetValue(ConfederationHeader);
        string rankingValue = row.GetValue(RankingHeader);
        string flagData = row.GetValue(FlagHeader)?.Trim() ?? "";

        if (importedNames.Contains(name))
            errors.Add(new ImportError(row.RowNumber, "El nombre del equipo está duplicado."));

        if (!Confederation.IsValidName(confederationName))
            errors.Add(new ImportError(row.RowNumber, "La confederación no es válida."));

        if (!int.TryParse(rankingValue, out int ranking) || ranking < Team.MinRanking || ranking > Team.MaxRanking)
            errors.Add(new ImportError(row.RowNumber, $"El ranking FIFA debe estar entre {Team.MinRanking} y {Team.MaxRanking}."));

        if (!string.IsNullOrWhiteSpace(flagData))
        {
            try
            {
                Convert.FromBase64String(flagData);
            }
            catch
            {
                errors.Add(new ImportError(row.RowNumber, "El formato de la bandera en esta fila es inválido."));
            }
        }

        return errors;
    }
}