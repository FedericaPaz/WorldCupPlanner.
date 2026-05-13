using Domain;

namespace IServices;

public interface ICrucesGeneratorService
{
    List<Cruce> GenerateNextRound(List<Cruce> currentRound, List<Stadium> stadiums, int seed);
    List<Cruce> GenerateRoundOf32(Fixture fixture, List<Stadium> estadios, int semilla);
}
