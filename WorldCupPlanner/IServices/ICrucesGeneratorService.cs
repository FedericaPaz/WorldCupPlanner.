using Domain;

namespace IServices;

public interface ICrucesGeneratorService
{
    List<Cruce> GenerateRoundOf32(List<Group> groups, List<Stadium> stadiums, int seed);
    List<Cruce> GenerateNextRound(List<Cruce> currentRound, List<Stadium> stadiums, int seed);
}
