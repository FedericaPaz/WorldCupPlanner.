using IServices;
using Domain;

namespace ServicesTests.Stubs
{
    public class SimulationServiceStub : ISimulationService
    {
        public (int homeGoals, int awayGoals) SimulateMatch(Match match, int seed)
        {
            throw new NotImplementedException();
        }

        public void SimulateGroupStage(List<Group> groups, int seed)
        {
            throw new NotImplementedException();
        }

        public void SimulateCruces(List<Cruce> cruces, int seed)
        {
            throw new NotImplementedException();
        }
    }
}