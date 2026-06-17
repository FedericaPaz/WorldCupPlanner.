---
name: solid-lsp
description: Verifica y aplica el principio LSP (Liskov Substitution Principle) asegurando que las implementaciones de interfaces y subclases respeten el contrato completo de la abstracción — sin restricciones adicionales en entradas, sin garantías menores en salidas, sin excepciones inesperadas y sin métodos vacíos o con NotImplementedException.
---

# SOLID — Liskov Substitution Principle (LSP)

## Qué es
Si `B` hereda de `A`, entonces cualquier lugar que use `A` debe poder usar `B` sin que el programa se rompa ni cambie su comportamiento esperado.

En términos simples: una subclase debe poder reemplazar a su clase base sin sorpresas.

---

## Señales de violación

- Una subclase lanza excepciones en métodos que la clase base no lanza
- Una subclase hace menos que la clase base (precondiciones más estrictas)
- Una subclase cambia el significado de un método heredado
- El código cliente hace `if (obj is SubclaseEspecifica)` para tratar casos especiales
- Una subclase tiene métodos heredados que no aplican y los deja vacíos o con `NotImplementedException`

---

## Cómo identificarlo en C#

```csharp
// VIOLA LSP — la subclase restringe el contrato de la base
public class SimulationEngine
{
    public virtual (int home, int away) SimulateMatch(int homeRanking, int awayRanking, int seed)
    {
        // retorna resultado válido siempre
    }
}

public class TournamentOnlyEngine : SimulationEngine
{
    public override (int home, int away) SimulateMatch(int homeRanking, int awayRanking, int seed)
    {
        if (homeRanking < 500)
            throw new ArgumentException("Ranking demasiado bajo"); // la base nunca lanzaba esto
        // ...
    }
}

// CORRECTO — ambas implementaciones respetan el contrato de ISimulationEngine
public class ProbabilisticSimulationEngine : ISimulationEngine
{
    public (int home, int away) SimulateMatch(int homeRanking, int awayRanking, int seed)
    {
        // calcula con probabilidades — siempre retorna resultado válido
    }
}

public class PureRandomSimulationEngine : ISimulationEngine
{
    public (int home, int away) SimulateMatch(int homeRanking, int awayRanking, int seed)
    {
        // calcula aleatoriamente — siempre retorna resultado válido
    }
}
```

---

## LSP con interfaces vs herencia

LSP aplica tanto a herencia de clases como a implementación de interfaces. Cuando una clase implementa una interfaz, debe respetar el contrato completo — no puede ignorar métodos, lanzar excepciones inesperadas ni cambiar precondiciones.

```csharp
// VIOLA LSP — implementa la interfaz pero no cumple el contrato
public class MockRepository : IMatchRepository
{
    public Match GetMatchById(int id)
    {
        throw new NotImplementedException(); // el contrato dice "retornar el partido"
    }
}

// CORRECTO — cumple el contrato aunque sea un stub para tests
public class MatchRepositoryStub : IMatchRepository
{
    private readonly List<Match> _matches = new();
    
    public Match GetMatchById(int id)
        => _matches.FirstOrDefault(m => m.IdMatch == id)
           ?? throw new KeyNotFoundException(); // consistente con el contrato
}
```

---

## Reglas de sustitución (contrato de Liskov)

| Regla | Descripción |
|-------|-------------|
| **Precondiciones** | La subclase no puede exigir más que la clase base — no puede ser más restrictiva en sus entradas |
| **Postcondiciones** | La subclase no puede garantizar menos que la clase base — debe cumplir al menos lo mismo |
| **Invariantes** | Las propiedades que siempre son verdaderas en la base deben seguir siéndolo en la subclase |
| **Excepciones** | La subclase no puede lanzar excepciones nuevas que el cliente no espera |

---

## Casos en este proyecto

### `ISimulationEngine` — LSP bien aplicado
`ProbabilisticSimulationEngine` y `PureRandomSimulationEngine` implementan `ISimulationEngine`. Ambas:
- Reciben los mismos parámetros
- Retornan siempre un resultado válido `(int home, int away)`
- No lanzan excepciones inesperadas
- Son intercambiables sin que el código cliente cambie

Esto permite que `Fixture.CreateEngine()` retorne cualquiera de las dos y el servicio que la usa no necesite saber cuál es.

### `ITeam` — LSP a verificar
`Team` implementa `ITeam`. Si en algún momento se creara una subclase de `Team` (por ejemplo `NationalTeam`), debe poder reemplazar a `Team` en todos los contextos — incluyendo `Match`, `Group`, `Standing`.

---

## Diferencia entre LSP y IE/SRP

- **SRP** habla de por qué cambia una clase
- **IE** habla de dónde vive la lógica
- **LSP** habla de cómo se comportan las clases en jerarquías — garantiza que la herencia o implementación de interfaces no rompe el código que depende de la abstracción

---

## Señal de buen diseño

Si podés agregar una nueva implementación de `ISimulationEngine` sin tocar ningún servicio existente, LSP y Low Coupling están bien aplicados. El motor nuevo simplemente se registra en `Program.cs` y funciona.
