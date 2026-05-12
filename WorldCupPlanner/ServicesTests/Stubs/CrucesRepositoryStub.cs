using Domain;
using IRepository;

namespace ServicesTests.Stubs;

public class CrucesRepositoryStub : ICrucesRepository
{
    private List<Cruce> _cruces = new List<Cruce>();
    public bool RemoveCalled { get; private set; }
    public List<Cruce> AddedCruces { get; private set; }
        
    public List<Cruce> GetAllCruces()
    {
        return _cruces.ToList();
    }

    public bool HasCruces()
    {
        return _cruces.Count > 0;
    }

    public void AddCruces(List<Cruce> cruces)
    {
        AddedCruces = cruces;
        _cruces.AddRange(cruces);
    }
        
    public void RemoveCruces()
    {
        RemoveCalled = true;
        _cruces.Clear();
    }
        
    public void SetCruces(List<Cruce> cruces)
    {
        _cruces = cruces.ToList();
    }
}