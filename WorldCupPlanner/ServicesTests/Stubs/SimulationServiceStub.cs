using IServices;
using Domain;
using System.Collections.Generic;

namespace ServicesTests.Stubs
{
    public class SimulationServiceStub : ISimulationService
    {
        public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
        {
            // deterministic result for tests
            return (2, 1);
        }

        public void SimulateGroupStage(List<Group> groups, int seed)
        {
            // For tests, simulate all matches in all groups with a fixed score
            foreach (var group in groups)
            {
                foreach (var match in group.MatchesInGroup)
                {
                    match.SimulateResult(2, 1);
                }
            }
        }

        public void SimulateCruces(List<Cruce> cruces, int seed)
        {
            // Not needed for current tests - keep no-op to satisfy interface
        }
    }
}
