using Domain;

namespace IRepository;

public interface ICrucesRepository
{
    void AddCruces(List<Cruce> cruces);
    List<Cruce> GetAllCruces();
    bool HasCruces();
    void RemoveCruces();
}