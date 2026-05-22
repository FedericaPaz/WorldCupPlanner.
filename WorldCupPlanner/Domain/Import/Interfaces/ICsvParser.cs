namespace Domain.Import.Interfaces;

public interface ICsvParser
{
    public ParsedCsv Parse(string csvContent);
}   