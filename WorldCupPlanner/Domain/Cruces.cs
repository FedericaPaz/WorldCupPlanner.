namespace Domain;

public class Cruce
{
    public string Nombre { get; private set; }
    public Match Partido { get; private set; }
    public TournamentPhase Fase { get; private set; }
    
    public Cruce(string nombre, Match partido, TournamentPhase fase)
    {
        if(string.IsNullOrEmpty(nombre))
            throw new ArgumentException("El nombre del cruce no puede ser vacío o nulo", "nombre");
        
        Nombre = nombre;
        Partido = partido;
        Fase = fase;
    }
}