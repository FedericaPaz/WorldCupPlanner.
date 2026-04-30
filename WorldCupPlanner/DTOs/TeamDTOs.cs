using Domain;

namespace DTOs;

public class TeamDTOs
{
    public class TeamDTO
    {
        public string? Name { get; set; }
        public string? ConfederationName { get; set; }
        public int FifaRanking { get; set; }
    }
}