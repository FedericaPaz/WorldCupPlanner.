using Domain;
namespace IServices;
using DTOs;

public interface IStadiumService
{
    void AddStadium(StadiumDTOs.CreateStadiumDTO request);
    void DeleteStadium(string stadiumName);
    List<StadiumDTOs.StadiumResponseDTO> GetAllStadiums();
}