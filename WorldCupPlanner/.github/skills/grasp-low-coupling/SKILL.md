# GRASP — Low Coupling

## Qué es
Low Coupling dice: una clase debe depender de pocas otras clases, y esas dependencias deben ser sobre abstracciones (interfaces), no sobre implementaciones concretas.

El acoplamiento alto hace que un cambio en una clase obligue a cambiar muchas otras.

---

## Tipos de acoplamiento

| Tipo | Descripción | Ejemplo |
|------|-------------|---------|
| **Acoplamiento de implementación** | Depender de una clase concreta en lugar de su interfaz | `new ProbabilisticSimulationEngine()` dentro de un servicio |
| **Acoplamiento de datos** | Exponer estado interno innecesariamente | Setters públicos cuando debería haber métodos de comportamiento |
| **Acoplamiento transitivo** | A conoce a B porque C lo necesita | Un servicio recibe una dependencia solo para pasársela a otro |
| **Acoplamiento de dominio cruzado** | Una entidad de dominio importa clases de infraestructura | `Fixture` instanciando `ProbabilisticSimulationEngine` directamente |

---

## Señales de violación

- `new ClaseConreta()` dentro de un servicio o entidad
- Constructor con muchas dependencias (más de 5-6 suele ser señal)
- Una clase importa namespaces de otra capa (dominio importando servicios)
- El mismo switch/if de tipos en varios lugares
- Una clase que cambia cada vez que se agrega una nueva implementación

---

## Cómo identificarlo en C#

```csharp
// VIOLA Low Coupling — acoplamiento a implementación concreta
public void SimulateMatch(int matchId, int seed)
{
    SimulateMatch(matchId, seed, new ProbabilisticSimulationEngine()); // acoplado
}

// CORRECTO — depende de la abstracción
public void SimulateMatch(int matchId, int seed)
{
    var fixture = _fixtureRepository.GetFixture();
    SimulateMatch(matchId, seed, fixture.CreateEngine()); // Fixture decide el motor
}

// VIOLA Low Coupling — dominio conoce infraestructura
private static readonly Dictionary<string, Func<ISimulationEngine>> _engines = new()
{
    ["Aleatorio puro"] = () => new PureRandomSimulationEngine(),   // clase de Services
    ["Probabilistico"]  = () => new ProbabilisticSimulationEngine() // clase de Services
};

// CORRECTO — registro desde afuera (Open/Closed + Low Coupling)
public static void RegisterEngine(string name, Func<ISimulationEngine> factory)
{
    _engines[name] = factory;
}
// En Program.cs:
Fixture.RegisterEngine("Aleatorio puro", () => new PureRandomSimulationEngine());
Fixture.RegisterEngine("Probabilistico",  () => new ProbabilisticSimulationEngine());
```

---

## Pasos para corregir

1. Reemplazar dependencias concretas por interfaces
2. Inyectar dependencias por constructor en lugar de instanciarlas internamente
3. Si una entidad necesita crear objetos de infraestructura, usar un patrón de registro (como `RegisterEngine`) para que la entidad no conozca las implementaciones
4. Cuando hay un switch de tipos repetido, crear una abstracción que lo elimine

---

## Relación con otros principios

- **IE** y **Low Coupling** se complementan: IE mueve lógica a la entidad correcta, Low Coupling asegura que esa entidad no se acopla a clases que no debería conocer
- **Indirection** es a veces la solución a High Coupling — agregar una capa intermedia que desacopla
- **SRP** ayuda a Low Coupling: una clase con una sola responsabilidad tiene menos razones para depender de otras clases

---

## Métricas de referencia

- Un constructor con más de 5-6 parámetros es señal de alto acoplamiento
- Si eliminar una clase obliga a cambiar 5+ otras clases, el acoplamiento es alto
- Si una entidad de dominio tiene `using Services;` o `using Repository;`, hay acoplamiento de capas
