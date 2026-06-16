---
name: grasp-indirection
description: >
  Aplica el principio GRASP Indirection introduciendo intermediarios (interfaces, fachadas, mappers, registros) entre clases que no deben conocerse directamente, para reducir acoplamiento entre capas o componentes.
---
# GRASP — Indirection

## Qué es
Indirection dice: cuando dos clases no deben conocerse directamente, introducir un intermediario que coordine la interacción entre ellas.

Es la solución de diseño para reducir acoplamiento directo entre componentes que necesitan colaborar.

---

## Cuándo aplicarlo

- Dos clases de capas diferentes necesitan comunicarse (dominio ↔ infraestructura)
- Una clase necesita crear objetos de otra sin conocer su tipo concreto
- Varios clientes necesitan la misma transformación antes de usar un objeto
- Una operación involucra múltiples pasos que no le pertenecen a ninguna clase en particular

---

## Formas comunes de Indirection en C#

### 1. Interfaces como intermediario
```csharp
// Sin indirection — acoplamiento directo a implementación
public class CruceService
{
    private readonly EfCrucesRepository _repo; // acoplado a EF
}

// Con indirection — desacoplado
public class CruceService
{
    private readonly ICrucesRepository _repo; // depende de la abstracción
}
```

### 2. Fachada como intermediario
```csharp
// Sin indirection — la página Blazor conoce múltiples servicios internos
@inject MatchCreationService _creator
@inject MatchResultProcessor _processor
@inject MatchSimulationOrchestrator _orchestrator

// Con indirection — la página solo conoce la fachada
@inject IMatchService MatchService // delega internamente a los tres
```

### 3. Registro como intermediario (evita acoplamiento de creación)
```csharp
// Sin indirection — Fixture conoce implementaciones concretas de Services
private static readonly Dictionary<string, Func<ISimulationEngine>> _engines = new()
{
    ["Aleatorio puro"] = () => new PureRandomSimulationEngine() // acoplado
};

// Con indirection — Fixture solo conoce la interfaz, el registro lo hace Program.cs
public static void RegisterEngine(string name, Func<ISimulationEngine> factory)
{
    _engines[name] = factory; // factory viene de afuera
}
```

### 4. Mapper como intermediario
```csharp
// Sin indirection — el servicio construye el DTO manualmente conociendo ambos lados
var dto = new MatchResponse { idMatch = match.IdMatch, ... };

// Con indirection — el mapper aísla la transformación
var dto = match.ToResponse(); // o MatchMapper.ToResponse(match)
```

---

## Casos en este proyecto

| Problema | Indirection aplicada |
|----------|---------------------|
| Dominio no debe conocer motores de simulación concretos | `Fixture.RegisterEngine` — registro desde `Program.cs` |
| Páginas Blazor no deben conocer detalles de implementación | Interfaces `IMatchService`, `ICruceService`, etc. |
| `CruceService` no debe conocer `EfCrucesRepository` | Interfaz `ICrucesRepository` |
| Transformación de entidades a DTOs | Mappers (`EntityMatchMapper`, `StandingMapper`, etc.) |

---

## Cuándo NO aplicar Indirection

- Cuando el intermediario no aporta valor y solo pasa datos sin transformar (middleman smell)
- Cuando la abstracción es prematura — si solo hay una implementación y no hay plan de tener más
- Cuando agrega complejidad sin reducir acoplamiento real

---

## Relación con otros principios

- **Low Coupling** es el objetivo; Indirection es la herramienta para lograrlo
- **IE** y Indirection colaboran: IE pone la lógica en el lugar correcto, Indirection evita que ese lugar se acople a clases que no debería conocer
- El patrón **Repository** es Indirection entre dominio e infraestructura de datos
- El patrón **Fachada** es Indirection entre clientes y un sistema complejo de clases
