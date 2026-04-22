using Domain;

namespace IRepository;

public interface IStadiumRepository
{
    void AddStadium(Stadium stadium);
    Stadium? GetStadium(string stadiumName);
    void DeleteStadium(Stadium stadium);
    List<Stadium> GetAllStadiums();
}