using Domain;

namespace IRepository;

public interface IStadiumRepository
{
    void AddStadium(Stadium stadium);
    List<Stadium> GetAll();
    Stadium? Get(string name);
    void RemoveStadium(string name);
}