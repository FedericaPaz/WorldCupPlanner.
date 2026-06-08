using Domain.Entities;
using IRepository;

namespace Repository;

public class StadiumRepository : IStadiumRepository
{
    private readonly WorldCupPlannerContext _context;

    public StadiumRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddStadium(Stadium stadium)
    {
        EnsureNotNull(stadium);
        _context.Stadiums.Add(stadium);
        _context.SaveChanges();
    }

    public Stadium? GetStadium(string stadiumName)
    {
        EnsureValidName(stadiumName);

        return _context.Stadiums.FirstOrDefault(s => s.StadiumName == stadiumName);
    }

    public void DeleteStadium(Stadium stadium)
    {
        EnsureNotNull(stadium);
        _context.Stadiums.Remove(stadium);
        _context.SaveChanges();
    }

    public List<Stadium> GetAllStadiums()
    {
        return _context.Stadiums.ToList();
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