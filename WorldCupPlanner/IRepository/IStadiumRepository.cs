using Domain;

namespace IRepository;

public interface IStadiumRepository
{
    void AddStadium(Stadium stadium);
    Stadium? GetStadium(string name);
    void UpdateStadium(Stadium stadium);
    void DeleteStadium(string name);
}