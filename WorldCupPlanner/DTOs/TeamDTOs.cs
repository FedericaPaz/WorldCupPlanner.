namespace DTOs;

public class TeamDTOs
{
    public class CreateTeamDTO
    {
        public string Name { get; set; }
        public string ConfederationName { get; set; }
        public int FifaRanking { get; set; }
    }

    public class TeamResponseDTO
    {
        public string Name { get; set; }
        public string ConfederationName { get; set; }
        public int FifaRanking { get; set; }
    }
}