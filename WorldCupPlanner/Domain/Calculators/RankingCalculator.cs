using Domain.Enums;

namespace Domain.Calculators;

public class RankingCalculator
{
    private const double GroupPhaseMultiplier = 1.0;
    private const double EliminatoryPhaseMultiplier = 1.5; 
    
    public int CalculateNewRanking(int currentRanking, int opponentRanking, double result, TournamentPhase tournamentPhase)
    {
        double probability = 1.0 / (1 + Math.Pow(10, (opponentRanking - currentRanking) / 1000.0));
        double multiplier = GetPhaseMultiplier(tournamentPhase);
        double newRanking = currentRanking + (30 * (result - probability)) * multiplier;
        return (int)Math.Round(newRanking, MidpointRounding.AwayFromZero);
    }
    
    private static double GetPhaseMultiplier(TournamentPhase tournamentPhase)
    {
        if (tournamentPhase == TournamentPhase.GroupStage) return GroupPhaseMultiplier;

        if (tournamentPhase == TournamentPhase.RoundOf32
            || tournamentPhase == TournamentPhase.RoundOf16
            || tournamentPhase == TournamentPhase.QuarterFinals
            || tournamentPhase == TournamentPhase.SemiFinals
            || tournamentPhase == TournamentPhase.Final)
            return EliminatoryPhaseMultiplier;

        throw new ArgumentException("Invalid tournament phase");
    }
}