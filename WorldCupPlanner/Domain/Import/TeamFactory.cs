using Domain.Entities;
using Domain.Import.Interfaces;
using Domain.Interfaces;
using System;

namespace Domain.Import;

public class TeamFactory : ITeamFactory
{
    private readonly IRankingCalculator _rankingCalculator;

    public TeamFactory(IRankingCalculator rankingCalculator)
    {
        _rankingCalculator = rankingCalculator ?? throw new ArgumentNullException(nameof(rankingCalculator));
    }

    public Team CreateTeam(string name, string confederationName, string rankingValue, string flagBase64 = "")
    {
        if (!int.TryParse(rankingValue, out int ranking))
            throw new ArgumentException("El ranking FIFA debe ser de tipo numérico.");

        return new Team(name, new Confederation(confederationName), ranking, flagBase64, _rankingCalculator);
    }
}