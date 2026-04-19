using Domain;

namespace IRepository;

public interface IStadiumRepository
{
    void AddStadium(Stadium stadium);
    Stadium? GetStadium(Stadium stadium);
    void UpdateStadium(Stadium stadium);
    void DeleteStadium(Stadium stadium);
    List<Stadium> GetAllStadiums();
}