using Domain.Entities;

namespace IServices;

public interface ICrucesGeneratorService
{
    List<Cruce> GenerateNextRound(List<Cruce> currentRound, List<Stadium> stadiums, int seed, int id);
    List<Cruce> GenerateRoundOf32(Fixture fixture, List<Stadium> estadios, int semilla, int id);
}
