using System.ComponentModel;

namespace Domain;

public class Cruce
{
    public string Nombre { get; private set; }
    public Match Partido { get; private set; }
    public TournamentPhase Phase { get; private set; }
    public Cruce? NextCruce { get; private set; }
    
    public Cruce(string nombre, Match partido, TournamentPhase fase)
    {
        if(string.IsNullOrEmpty(nombre))
            throw new ArgumentException("El nombre del cruce no puede ser vacío o nulo", "nombre");
        
        if( partido == null)
            throw new ArgumentNullException("partido", "partido");
        
        Nombre = nombre;
        Partido = partido;
        Phase = fase;
    }

    public void DecideNextCruce(Cruce cruce)
    {
        if(cruce == this)
            throw new InvalidOperationException("El cruce no puede ser igual a sí mismo");
        NextCruce = cruce;
    }
}