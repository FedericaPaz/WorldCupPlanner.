using System.Linq;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;
    
    private const int MinCapacity = 20000;
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;

    public StadiumService(IStadiumRepository repository)
    {
        _stadiumRepository = repository;
    }

    public void AddStadium(StadiumRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.stadiumName))
            throw new ArgumentException("El nombre es requerido", nameof(request.stadiumName));
        if (string.IsNullOrWhiteSpace(request.stadiumCity))
            throw new ArgumentException("La ciudad es requerida", nameof(request.stadiumCity));
        if (request.stadiumCity.Length > MaxCityLength)
            throw new ArgumentException("Ciudad demasiado larga");
        if (request.stadiumDescription != null && request.stadiumDescription.Length > MaxDescriptionLength)
            throw new ArgumentException("La descripción es demasiado larga");
        if (request.locativeCapacity < MinCapacity)
            throw new ArgumentException($"La capacidad locativa debe ser al menos {MinCapacity}");
        
        var existingStadium = _stadiumRepository.GetStadium(request.stadiumName);
        if (existingStadium != null)
            throw new InvalidOperationException("El estadio ya existe");
        
        _stadiumRepository.AddStadium(request.ToEntity());
    }

    public void DeleteStadium(string name)
    {
        var stadium = _stadiumRepository.GetStadium(name);
        if (stadium == null)
            throw new InvalidOperationException("Estadio no encontrado");

        _stadiumRepository.DeleteStadium(stadium);
    }

    public List<StadiumRequest> GetAllStadiums()
    {
        return _stadiumRepository.GetAllStadiums().Select(s => s.ToRequest()).ToList();
    }

    public StadiumRequest? GetStadium(string stadiumName)
    {
        if(string.IsNullOrWhiteSpace(stadiumName))
            return null;
        var stadium = _stadiumRepository.GetStadium(stadiumName);
        return stadium is null ? null : stadium.ToRequest();
    }
}