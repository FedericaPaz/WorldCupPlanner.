using Domain.Entities;
using IRepository;

namespace Repository;

public class CrucesRepository : ICrucesRepository
{
    private WorldCupPlannerContext _context;

    public CrucesRepository(WorldCupPlannerContext context)
    {
        _context = context;
    }

    public void AddCruces(List<Cruce> cruces)
    {
        if (cruces is null)
            throw new ArgumentNullException();
        _context.Cruces.AddRange(cruces);
        _context.SaveChanges();
    }
    
   /* public void AddCruces(List<Cruce> cruces)
    {
        foreach (var c in cruces)
            _cruces.Add(c);
    }*/

   public List<Cruce> GetAllCruces()
   {
       return _context.Cruces.ToList();
   }
   
    /*public List<Cruce> GetAllCruces()
    {
        return _cruces;
    }*/
    
    /*public bool HasCruces()
    {
        return _cruces.Count > 0;
    }*/

    public bool HasCruces()
    {
        return _context.Cruces.Any();
    }

    public void RemoveCruces()
    {
        _context.Cruces.RemoveRange(_context.Cruces);
        _context.SaveChanges();
    }

   /* public void RemoveCruces()
    {
        _cruces.Clear();
    }*/
}