using System.Linq;
using DTOs;
using IRepository;
using IServices;
using Services.Mappers;

namespace Services;

public class StadiumService : IStadiumService
{
    private readonly IStadiumRepository _stadiumRepository;

    public StadiumService(IStadiumRepository repository)
    {
        _stadiumRepository = repository;
    }

    public void AddStadium(StadiumRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "La solicitud es requerida");
        if (string.IsNullOrWhiteSpace(request.stadiumName))
            throw new ArgumentException("El nombre es requerido", nameof(request.stadiumName));
        if (string.IsNullOrWhiteSpace(request.stadiumCity))
            throw new ArgumentException("La ciudad es requerida", nameof(request.stadiumCity));

        var existingStadium = _stadiumRepository.GetStadium(request.stadiumName);
        if (existingStadium != null)
            throw new InvalidOperationException("El estadio ya existe");

        var newStadium = request.ToEntity();
        _stadiumRepository.AddStadium(newStadium);
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