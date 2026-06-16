using Domain.Entities;
using Domain.Import.Interfaces;
using Domain.Interfaces;

namespace DomainTests.Stubs;

public class TeamFactoryStub : ITeamFactory
{
    private readonly IRankingCalculator _rankingCalculator;

    // Aceptamos un calculador opcional; si no viene, usamos un comportamiento stub básico
    public TeamFactoryStub(IRankingCalculator? rankingCalculator = null)
    {
        _rankingCalculator = rankingCalculator ?? new RankingCalculatorStub();
    }

    // Agregamos el parámetro obligatorio de la interfaz con su valor por defecto
    public Team CreateTeam(string name, string confederationName, string rankingValue, string flagBase64 = "")
    {
        int ranking = 0;
        int.TryParse(rankingValue, out ranking);

        return new Team(
            name,
            new Confederation(confederationName),
            ranking,
            flagBase64, // Pasamos la bandera al constructor del dominio
            _rankingCalculator // Pasamos el calculador para cumplir con el invariante
        );
    }
}

// Clase de soporte rápida para que tu Stub compile sin dependencias de infraestructura
internal class RankingCalculatorStub : IRankingCalculator
{
    public int CalculateNewRanking(int currentRanking, int opponentRanking, double result, Domain.Enums.TournamentPhase phase)
    {
        return currentRanking; // No altera el ranking en los escenarios de test de importación
    }
}