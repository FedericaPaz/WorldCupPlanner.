using Domain;

namespace DTOs;

public class TeamDTOs
{
    public class CreateTeamDTO
    {
        public string Name { get; set; }
        public string ConfederationName { get; set; }
        public int FifaRanking { get; set; }
        
        public Team ToEntity()
        {
            var confederation = new Confederation(ConfederationName);
            return new Team(Name, confederation, FifaRanking);
        }
    }

    public class TeamResponseDTO
    {
        public string Name { get; set; }
        public string ConfederationName { get; set; }
        public int FifaRanking { get; set; }
        
        public static TeamResponseDTO FromEntity(Team team)
        {
            return new TeamResponseDTO
            {
                Name = team.Name,
                ConfederationName = team.Confederation.Name,
                FifaRanking = team.FifaRanking
            };
        }
    }
}