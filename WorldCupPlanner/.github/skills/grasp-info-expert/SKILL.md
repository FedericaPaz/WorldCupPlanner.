# GRASP — Information Expert

## Qué es
Information Expert dice: la responsabilidad de realizar una operación debe recaer sobre la clase que tiene la información necesaria para llevarla a cabo.

Si para computar algo necesitás datos de una clase, esa clase debería hacer el cómputo — no un servicio externo que le extrae los datos.

---

## Señales de violación

- Un servicio extrae datos de una entidad para computar algo y le devuelve el resultado
- Una entidad expone sus datos crudos y otra clase hace el trabajo
- El mismo dato se extrae en varios lugares para hacer siempre la misma operación
- Constantes de validación duplicadas entre la entidad y el servicio que la usa
- Un método privado en un servicio que solo accede a propiedades de una entidad ajena

---

## Cómo identificarlo en C#

```csharp
// VIOLA IE — el servicio extrae datos de Team para calcular el ranking
public void UpdateRankingsAfterMatch(Match match)
{
    int newHomeRanking = _calculator.CalculateNewRanking(
        homeTeam.CurrentRanking,   // dato de Team
        awayTeam.CurrentRanking,   // dato de Team
        match.GetResultForHomeTeam(), // dato de Match
        phase);
    homeTeam.UpdateCurrentRanking(newHomeRanking);
}

// CORRECTO — Team es el experto en su propio ranking
public void RecalcularRanking(int rankingRival, double resultado, TournamentPhase fase)
{
    int nuevoRanking = _calculator.CalculateNewRanking(_currentRanking, rankingRival, resultado, fase);
    UpdateCurrentRanking(nuevoRanking);
}
```

---

## Pasos para corregir

1. Identificar qué clase tiene los datos necesarios para la operación
2. Mover el método a esa clase
3. Si la operación necesita un colaborador (calculadora, helper), la entidad lo instancia internamente
4. El servicio pasa a llamar al método de la entidad en lugar de extraer sus datos

---

## Casos frecuentes en este proyecto

| Operación | Experto correcto | Anti-patrón a evitar |
|-----------|-----------------|----------------------|
| Calcular ranking de equipo | `Team` | `RankingService` extrayendo `CurrentRanking` |
| Calcular standings de un grupo | `Group` | `StandingCalculator` externo |
| Obtener primeros/segundos/terceros | `Fixture` | `CrucesGeneratorService` iterando grupos |
| Resolver motor de simulación | `Fixture` | Método `ResolveEngine` duplicado en servicios |
| Lockear partidos al avanzar fase | `Fixture` | `CruceService` iterando grupos desde afuera |
| Saber si un equipo puede agregarse a un grupo | `Group` | `FixtureGenerator` con método `CanAddTeamToGroup` |
| Validar capacidad y nombre de estadio | `Stadium` | `StadiumService` con constantes duplicadas |
| Saber si un usuario puede exportar | `User` | `ExportService` verificando roles directamente |

---

## Regla práctica

Antes de escribir lógica en un servicio, preguntarse:
> ¿Quién tiene los datos que necesito para esta operación?

Si la respuesta es una entidad de dominio, la lógica va ahí.

---

## Lo que NO viola IE

- Un servicio que orquesta: obtiene entidades del repo, les dice qué hacer, persiste. Eso es correcto.
- Un servicio que valida existencia en base de datos antes de crear una entidad. La entidad no puede hacer eso.
- Mappers y DTOs que transforman datos para la capa de presentación.
- Constantes de infraestructura (longitudes máximas de nombre en BD) que no son reglas de negocio.
