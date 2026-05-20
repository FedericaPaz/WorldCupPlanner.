using Domain.Enums;

namespace Domain.Calculators;

public class RankingCalculator
{
    
    public int CalculateNewRanking(int currentRanking, int opponentRanking, double result, TournamentPhase tournamentPhase)
    {
        double probability = 1.0 / (1 + Math.Pow(10, (opponentRanking - currentRanking) / 1000.0));
        double multiplier;
        if(tournamentPhase == TournamentPhase.GroupStage)
            multiplier = 1.0;
        else
            multiplier = 1.5;
        double newRanking = currentRanking + (30 * (result - probability)) * multiplier;
        return (int)Math.Round(newRanking, MidpointRounding.AwayFromZero);
    }
}