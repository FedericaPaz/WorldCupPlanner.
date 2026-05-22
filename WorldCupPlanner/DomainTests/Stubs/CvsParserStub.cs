using Domain.Import;
using Domain.Import.Interfaces;

namespace DomainTests.Stubs;

public class CsvParserStub : ICsvParser
{
    public ParsedCsv Parse(string csvContent)
    {
        return new ParsedCsv(new List<string>());
    }
}