using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Builders;

public class FixtureBuilder
{
    private readonly TournamentConfig _config;
    private readonly IDateTimeMatch _clock;

    private const int TeamsPerGroup = 4;
    private const int StartHour = 14;
    private const int HoursBetweenMatches = 4;
    private const int GroupsPerBatch = 6;

    public FixtureBuilder(TournamentConfig config, IDateTimeMatch clock)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Fixture BuildGroupsStageFixture(List<Team> teams, List<Stadium> stadiums, ref int nextMatchId)
    {
        if (teams == null) throw new ArgumentNullException(nameof(teams));
        if (stadiums == null) throw new ArgumentNullException(nameof(stadiums));

        if (teams.Count != 48)
            throw new InvalidOperationException("Se necesitan 48 equipos para generar el fixture.");
        if (stadiums.Count < 4)
            throw new InvalidOperationException("Se necesitan al menos 4 estadios para generar el fixture.");

        var fixture = new Fixture(_config);
        fixture.InitializeGroups();

        var orderedTeams = OrderTeamsWithTieBreak(teams);
        DistributeTeamsToGroups(orderedTeams, fixture.Groups.ToList());
        GenerateMatches(fixture, stadiums, ref nextMatchId);

        fixture.AdvanceToPhase(TournamentPhase.GroupStage);
        return fixture;
    }

    private List<Team> OrderTeamsWithTieBreak(List<Team> teams)
    {
        var result = new List<Team>();
        var groupedByRanking = teams
            .GroupBy(t => t.FifaRanking)
            .OrderByDescending(g => g.Key);

        foreach (var rankingGroup in groupedByRanking)
        {
            var tied = rankingGroup.ToList();
            var shuffled = tied.Count > 1 ? _config.Shuffle(tied, _config.FixtureSeed) : tied;
            result.AddRange(shuffled);
        }
        return result;
    }

    private void DistributeTeamsToGroups(List<Team> orderedTeams, List<Group> groups)
    {
        bool success = TryAssignAll(orderedTeams, 0, groups);
        if (!success)
            throw new InvalidOperationException("No se pudo distribuir los equipos respetando las restricciones de confederación.");
    }

    private bool TryAssignAll(List<Team> teams, int index, List<Group> groups)
    {
        if (index == teams.Count) return true;

        var team = teams[index];
        int roundRobinGroup = index % groups.Count;

        for (int offset = 0; offset < groups.Count; offset++)
        {
            int groupIndex = (roundRobinGroup + offset) % groups.Count;
            var group = groups[groupIndex];

            if (group.TeamsInGroup.Count >= TeamsPerGroup) continue;
            if (!group.CanAddTeam(team)) continue;

            group.AddTeamToGroup(team);
            if (TryAssignAll(teams, index + 1, groups)) return true;
            group.RemoveTeamFromGroup(team);
        }
        return false;
    }

    private void GenerateMatches(Fixture fixture, List<Stadium> stadiums, ref int nextMatchId)
    {
        var orderedStadiums = stadiums
            .OrderBy(s => NormalizeName(s.StadiumName))
            .ToList();

        var groups = fixture.Groups.ToList();
        int matchCounter = 0;

        foreach (var (groupIndex, group) in groups.Select((g, i) => (i, g)))
            matchCounter = GenerateGroupMatches(group, orderedStadiums, groupIndex, matchCounter, ref nextMatchId);
    }

    private int GenerateGroupMatches(Group group, List<Stadium> orderedStadiums, int groupIndex, int matchCounter, ref int nextMatchId)
    {
        var teams = group.TeamsInGroup.ToList();
        var matchDates = CalculateMatchDatesForGroup(groupIndex);
        var matchups = GetGroupRoundRobinMatchups();

        for (int i = 0; i < matchups.Length; i++)
        {
            var stadium = orderedStadiums[matchCounter % orderedStadiums.Count];
            var match = new Match(
                nextMatchId++,
                matchDates[i],
                stadium,
                teams[matchups[i].home],
                teams[matchups[i].away],
                group.GroupLetter,
                _clock);
            group.AddMatchToGroup(match);
            matchCounter++;
        }
        return matchCounter;
    }

    private static (int home, int away)[] GetGroupRoundRobinMatchups()
    {
        return new (int home, int away)[]
        {
            (0, 3), (1, 2),
            (0, 2), (1, 3),
            (0, 1), (2, 3),
        };
    }

    private DateTime[] CalculateMatchDatesForGroup(int groupIndex)
    {
        int batchIndex = groupIndex / GroupsPerBatch;
        int positionInBatch = groupIndex % GroupsPerBatch;

        DateTime round1Date = CalculateRoundDate(batchIndex, roundOffset: 0, positionInBatch);
        DateTime round2Date = CalculateRoundDate(batchIndex, roundOffset: _config.DaysBetweenMatchdays, positionInBatch);
        DateTime round3Date = CalculateRoundDate(batchIndex, roundOffset: _config.DaysBetweenMatchdays * 2, positionInBatch);

        return new[] { round1Date, round1Date, round2Date, round2Date, round3Date, round3Date };
    }

    private DateTime CalculateRoundDate(int batchIndex, int roundOffset, int positionInBatch)
    {
        int daysFromStart = batchIndex + roundOffset;
        int hour = StartHour + (positionInBatch % _config.MaxMatchesPerDay) * HoursBetweenMatches;
        return _config.TournamentStartDate.Date.AddDays(daysFromStart).AddHours(hour);
    }

    private static string NormalizeName(string name)
    {
        var lower = name.ToLowerInvariant();
        var normalized = string.Concat(lower.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));
        return string.Join(" ", normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }
}
