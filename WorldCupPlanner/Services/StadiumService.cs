using Domain;
using DTOs;
using IRepository;
using IServices;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;

    public StadiumService(IStadiumRepository repository)
    {
        _stadiumRepository = repository;
    }

    public void AddStadium(StadiumDTOs.CreateStadiumDTO request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "Request is required");
        if (string.IsNullOrWhiteSpace(request.stadiumName))
            throw new ArgumentException("Name is required", nameof(request.stadiumName));
        if (string.IsNullOrWhiteSpace(request.stadiumCity))
            throw new ArgumentException("City is required", nameof(request.stadiumCity));

        var existingStadium = _stadiumRepository.GetStadium(request.stadiumName);
        if (existingStadium != null)
            throw new InvalidOperationException("Stadium already exists");

        var newStadium = request.ToEntity();
        _stadiumRepository.AddStadium(newStadium);
    }

    public void DeleteStadium(string name)
    {
        var stadium = _stadiumRepository.GetStadium(name);
        if (stadium == null)
            throw new InvalidOperationException("Stadium not found");

        _stadiumRepository.DeleteStadium(stadium);
    }

    public List<StadiumDTOs.StadiumResponseDTO> GetAllStadiums()
    {
        return _stadiumRepository.GetAllStadiums().Select(StadiumDTOs.StadiumResponseDTO.FromEntity).ToList();
    }

    public StadiumDTOs.StadiumResponseDTO? GetStadium(string stadiumName)
    {
        var stadium = _stadiumRepository.GetStadium(stadiumName);
        return StadiumDTOs.StadiumResponseDTO.FromEntity(stadium);
    }
}