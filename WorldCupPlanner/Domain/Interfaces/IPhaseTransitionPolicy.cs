using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IPhaseTransitionPolicy
    {
        TournamentPhase GetNextPhase(TournamentPhase currentPhase);
    }
}
