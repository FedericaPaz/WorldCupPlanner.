# Skill: Analizador de SRP para el Namespace de Servicios

## Descripción
Analiza las clases de servicio en C# de un namespace dado para detectar violaciones del Principio de Responsabilidad Única (SRP) — tanto clases que hacen demasiado **como** clases sobre-fragmentadas (demasiadas clases pequeñas que representan una única responsabilidad conceptual). Sugiere código refactorizado que consolida o divide las clases según corresponda.

## Cuándo usar esta skill
Usá esta skill cuando el usuario pida:
- Analizar el cumplimiento de SRP en un namespace o carpeta
- Detectar clases de servicio sobre-fragmentadas o sobrecargadas
- Sugerir cómo consolidar o dividir servicios
- Revisar cohesión y acoplamiento en la capa de servicios

## Instrucciones para el Agente

### Paso 1: Descubrir las clases
Listar todos los archivos `.cs` en el namespace objetivo (por ejemplo `Services/`). Excluir archivos de prueba, mappers, stubs y archivos generados (`obj/`).

### Paso 2: Agrupar clases por entidad conceptual
Agrupar las clases descubiertas según la entidad de dominio o concepto al que pertenecen. Por ejemplo:
- `CruceService`, `CrucesGeneratorService`, `CrucesSimulationService` → todas se relacionan con **Cruces**
- `FixtureService`, `FixtureGenerator` → todas se relacionan con **Fixture**

Un grupo con más de una clase para el mismo concepto es candidato a consolidación.

### Paso 3: Detectar violaciones

#### Sobre-fragmentación (demasiadas clases pequeñas para un mismo concepto)
Marcar cuando:
- Dos o más clases comparten el mismo sustantivo de dominio en su nombre (por ejemplo `CruceService` + `CrucesGeneratorService`)
- Las clases se inyectan juntas como dependencias en otros servicios
- Cada clase tiene menos de 3 métodos públicos

#### Sobrecarga (una clase hace demasiado)
Marcar cuando:
- Una clase tiene más de 5 dependencias inyectadas
- Una clase tiene métodos públicos que pertenecen a responsabilidades claramente distintas (por ejemplo persistencia + lógica de negocio + notificaciones)
- El nombre de la clase es genérico (`Manager`, `Helper`, `Util`, `Handler`)

### Paso 4: Reportar hallazgos
Por cada violación, mostrar:
```
[TIPO DE VIOLACIÓN] NombreClase
Motivo: <por qué viola SRP>
Sugerencia: <descripción breve del arreglo>
```

### Paso 5: Generar código refactorizado
Por cada grupo marcado, generar una clase consolidada o dividida con:
- Una única responsabilidad clara expresada en un comentario de resumen al inicio
- Todos los métodos relevantes de las clases originales fusionados o redistribuidos
- Constructor actualizado con solo las dependencias necesarias para esa responsabilidad
- Las mismas firmas de métodos que las originales para no romper los llamadores

### Paso 6: Mostrar el antes y el después
Siempre mostrar:
1. La(s) clase(s) original(es) fragmentadas o sobrecargadas
2. La(s) clase(s) refactorizada(s) propuesta(s)
3. Un párrafo de justificación explicando cómo el refactor mejora SRP, cohesión y mantenibilidad

## Ejemplo

**Entrada:** Analizar SRP en el namespace Services de WorldCupPlanner

**Salida esperada:**

```
[SOBRE-FRAGMENTACIÓN] CruceService + CrucesGeneratorService + CrucesSimulationService
Motivo: Tres clases manejan el mismo concepto de dominio (Cruces). CruceService expone
operaciones de consulta, CrucesGeneratorService maneja la lógica de creación y
CrucesSimulationService maneja la simulación — pero las tres siempre se usan juntas
y comparten la misma entidad de dominio. Esto genera indirección innecesaria y obliga
a los llamadores a inyectar tres dependencias en lugar de una.
Sugerencia: Consolidar en un único CruceService con tres secciones privadas bien
diferenciadas: consultas, generación y simulación.
```

```csharp
// Antes: tres clases separadas inyectadas juntas
public class AlgunComponenteBlazor
{
    private readonly CruceService _cruceService;
    private readonly CrucesGeneratorService _generador;
    private readonly CrucesSimulationService _simulador;
    // ...
}

// Después: servicio único y cohesivo
/// <summary>
/// Gestiona todas las operaciones relacionadas con Cruces: consultas, generación
/// del sorteo de emparejamientos y simulación de resultados para la fase eliminatoria.
/// Responsabilidad única: orquestar el ciclo de vida del cuadro eliminatorio.
/// </summary>
public class CruceService : ICruceService
{
    private readonly ICrucesRepository _crucesRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IStadiumRepository _stadiumRepository;
    private readonly IRankingService _rankingService;
    private readonly ILogService _logService;

    public CruceService(
        ICrucesRepository crucesRepository,
        IMatchRepository matchRepository,
        IStadiumRepository stadiumRepository,
        IRankingService rankingService,
        ILogService logService)
    { ... }

    // --- Consultas ---
    public List<CruceResponse> GetAllCruces() { ... }
    public CruceResponse GetCruceByName(string name) { ... }

    // --- Generación ---
    public void GenerarCruces(int semilla) { ... }

    // --- Simulación ---
    public void SimularCruces(int semilla, ISimulationEngine motor) { ... }
}
```

## Notas
- No marcar las clases mapper — son intencionalmente pequeñas y cohesivas por diseño.
- No marcar las clases de motor (`ProbabilisticSimulationEngine`, `PureRandomSimulationEngine`) — implementan el patrón Strategy y SRP se cumple por diseño.
- Al consolidar, preservar todas las interfaces existentes para no romper los registros de inyección de dependencias.
- Siempre incluir sugerencias de actualización del `Program.cs` cuando una clase se elimina o renombra.
