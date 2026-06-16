using DTOs.Stadium;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IMatchRepository _matchRepository;

    public StadiumService(IStadiumRepository repository, IMatchRepository matchRepository)
    {
        _stadiumRepository = repository;
        _matchRepository = matchRepository;
    }

    public void AddStadium(StadiumRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        EnsureStadiumDoesNotExist(request.stadiumName);
        _stadiumRepository.AddStadium(request.ToEntity());
    }

    public void DeleteStadium(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nombre requerido.");

        if (_matchRepository.GetAllMatches().Any(m => m.Stadium.StadiumName == name))
            throw new InvalidOperationException(
                "No se puede eliminar el estadio porque tiene partidos asignados. Eliminá el fixture primero.");

        var stadium = _stadiumRepository.GetStadium(name)
                      ?? throw new InvalidOperationException("Estadio no encontrado.");

        _stadiumRepository.DeleteStadium(stadium);
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
            throw new ArgumentException("Nombre requerido.");

        var stadium = _stadiumRepository.GetStadium(stadiumName);
        return stadium?.ToRequest();
    }

    private void EnsureStadiumDoesNotExist(string stadiumName)
    {
        if (_stadiumRepository.GetStadium(stadiumName) != null)
            throw new InvalidOperationException("El estadio ya existe.");
    }
}