using Domain;
namespace IServices;

public interface IStadiumService
{
    void AddStadium(string stadiumName, string stadiumCity, string? stadiumDescription, int locativeCapacity);
    void DeleteStadium(string stadiumName);
    List<Stadium> GetAllStadiums();
}