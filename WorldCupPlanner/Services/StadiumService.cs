using DTOs.Stadium;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IMatchRepository _matchRepository;
    
    private const int MinCapacity = 20000;
    private const int MaxNameLength = 80;
    private const int MaxCityLength = 60;
    private const int MaxDescriptionLength = 400;

    public StadiumService(IStadiumRepository repository, IMatchRepository matchRepository)
    {
        _stadiumRepository = repository;
        _matchRepository = matchRepository;
    }

    public void AddStadium(StadiumRequest request)
    {
        EnsureValidRequest(request);
        EnsureStadiumDoesNotExist(request.stadiumName);
        _stadiumRepository.AddStadium(request.ToEntity());
    }

    private void EnsureValidRequest(StadiumRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
        EnsureValidName(request.stadiumName);
        EnsureValidCity(request.stadiumCity);
        EnsureValidDescription(request.stadiumDescription);
        EnsureValidCapacity(request.locativeCapacity);
    }

    private void EnsureValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es requerido");

        if (name.Length > MaxNameLength)
            throw new ArgumentException("Nombre demasiado largo");
    }

    private void EnsureValidCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("La ciudad es requerida");

        if (city.Length > MaxCityLength)
            throw new ArgumentException("Ciudad demasiado larga");
    }
    
    private void EnsureValidDescription(string? description)
    {
        if (description != null && description.Length > MaxDescriptionLength)
            throw new ArgumentException("La descripción es demasiado larga");
    }
    
    private void EnsureValidCapacity(int capacity)
    {
        if (capacity < MinCapacity)
            throw new ArgumentException($"La capacidad locativa debe ser al menos {MinCapacity}");
    }
    
    private void EnsureStadiumDoesNotExist(string stadiumName)
    {
        var existingStadium = _stadiumRepository.GetStadium(stadiumName);

        if (existingStadium != null)
            throw new InvalidOperationException("El estadio ya existe");
    }
    
   
    public void DeleteStadium(string name)
    {
        EnsureValidStadiumName(name);

        if (_matchRepository.GetAllMatches().Any(m => m.Stadium.StadiumName == name))
            throw new InvalidOperationException(
                "No se puede eliminar el estadio porque tiene partidos asignados. Eliminá el fixture primero.");

        var stadium = _stadiumRepository.GetStadium(name);
        if (stadium == null)
            throw new InvalidOperationException("Estadio no encontrado");

        _stadiumRepository.DeleteStadium(stadium);
    }
    
    private void EnsureValidStadiumName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nombre requerido");
    }

    public List<StadiumRequest> GetAllStadiums()
    {
        return _stadiumRepository
            .GetAllStadiums()
            .Select(s => s.ToRequest())
            .ToList();
    }

    public StadiumRequest? GetStadium(string stadiumName)
    {
        if (string.IsNullOrWhiteSpace(stadiumName))
            throw new ArgumentException("Nombre requerido");

        var stadium = _stadiumRepository.GetStadium(stadiumName);
        return stadium?.ToRequest();
    }
}