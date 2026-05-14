namespace Domain;

public interface ICsvParser
{
    public ParsedCsv Parse(string csvContent);
}   