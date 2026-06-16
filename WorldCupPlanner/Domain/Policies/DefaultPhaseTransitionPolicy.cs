using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Policies;

public class DefaultPhaseTransitionPolicy : IPhaseTransitionPolicy
{
    public TournamentPhase GetNextPhase(TournamentPhase currentPhase)
    {
        return currentPhase switch
        {
            TournamentPhase.RoundOf32 => TournamentPhase.RoundOf16,
            TournamentPhase.RoundOf16 => TournamentPhase.QuarterFinals,
            TournamentPhase.QuarterFinals => TournamentPhase.SemiFinals,
            TournamentPhase.SemiFinals => TournamentPhase.Final,
            TournamentPhase.Final => throw new InvalidOperationException("La final no tiene siguiente fase"),
            TournamentPhase.ThirdPlace => throw new InvalidOperationException("El partido por el tercer puesto no tiene siguiente fase"),
            _ => throw new InvalidOperationException($"La fase {currentPhase} no tiene siguiente definida")
        };
    }
}
