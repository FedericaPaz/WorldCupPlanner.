using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IRankingCalculator
    {
        int CalculateNewRanking(int currentRanking, int opponentRanking, double result, TournamentPhase tournamentPhase);
    }
}
