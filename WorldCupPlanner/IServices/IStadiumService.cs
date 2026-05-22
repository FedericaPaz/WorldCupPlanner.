using DTOs.Stadium;

namespace IServices;

public interface IStadiumService
{
    void AddStadium(StadiumRequest request);
    void DeleteStadium(string stadiumName);
    List<StadiumRequest> GetAllStadiums();
    StadiumRequest? GetStadium(string stadiumName);
}