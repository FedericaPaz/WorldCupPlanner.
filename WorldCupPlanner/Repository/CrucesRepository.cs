using Domain;
using IRepository;

namespace Repository;

public class CrucesRepository : ICrucesRepository
{
    private List<Cruce> _cruces = new();
    
    public void AddCruces(List<Cruce> cruces)
    {
        foreach (var c in cruces)
            _cruces.Add(c);
    }

    public List<Cruce> GetAllCruces()
    {
        return _cruces;
    }

    public bool HasCruces()
    {
        throw new NotImplementedException();
    }

    public void RemoveCruces()
    {
        throw new NotImplementedException();
    }
}