using Domain.Entities;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private readonly List<Stadium> _stadiums = new();

    public void AddStadium(Stadium stadium)
    {
        EnsureNotNull(stadium);
        _stadiums.Add(stadium);
    }

    public Stadium? GetStadium(string stadiumName)
    {
        EnsureValidName(stadiumName);

        return _stadiums.FirstOrDefault(s =>
            s.StadiumName.Equals(stadiumName, StringComparison.OrdinalIgnoreCase));
    }

    public void DeleteStadium(Stadium stadium)
    {
        EnsureNotNull(stadium);
        _stadiums.Remove(stadium);
    }

    public List<Stadium> GetAllStadiums()
    {
        return _stadiums.ToList();
    }

    private void EnsureNotNull(Stadium stadium)
    {
        if (stadium == null)
            throw new ArgumentNullException(nameof(stadium));
    }

    private void EnsureValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nombre requerido");
    }
}