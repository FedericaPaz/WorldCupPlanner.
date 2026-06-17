using IRepository;
using Domain.Entities;

namespace Services;

public static class TeamGenerator
{
    public static void GenerateMissingTeams(ITeamRepository repository, TournamentConfig config)
    {
        var random = new Random(config.FillingSeed);

        var confederations = new List<(string name, int limit)>
        {
            ("UEFA",     16 - CountConfederation(repository, "UEFA")),
            ("CONMEBOL",  7 - CountConfederation(repository, "CONMEBOL")),
            ("CONCACAF",  7 - CountConfederation(repository, "CONCACAF")),
            ("CAF",       9 - CountConfederation(repository, "CAF")),
            ("AFC",       8 - CountConfederation(repository, "AFC")),
            ("OFC",       1 - CountConfederation(repository, "OFC"))
        };

        foreach (var conf in confederations)
        {
            int confIndex = 1;
            for (int i = 0; i < conf.limit; i++)
            {
                string name = conf.name + "_" + confIndex.ToString("D2");
                while (repository.Get(name) != null)
                {
                    confIndex++;
                    name = conf.name + "_" + confIndex.ToString("D2");
                }
                confIndex++;

                int ranking = random.Next(Team.MinRanking, Team.MaxRanking + 1);
                repository.Add(new Team(name, new Confederation(conf.name), ranking));
            }
        }
    }

    private static int CountConfederation(ITeamRepository repository, string confederationName)
    {
        return repository.GetAll()
            .Count(t => t.Confederation.Name == confederationName);
    }
}
