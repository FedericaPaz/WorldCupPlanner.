---
name: ef-repository-tests
description: >
  Adapta tests de repositorios que usan almacenamiento en memoria (listas internas) para que usen
  Entity Framework Core con el proveedor InMemory. Úsala siempre que el usuario tenga tests de
  repositorios con instanciación directa (new Repository()) o con listas internas y quiera migrarlos
  a EF Core, o cuando el repositorio de producción ya use DbContext pero los tests no. También
  útil cuando el usuario menciona "tests con EF", "InMemory provider", "DbContext en tests",
  "repositorio con contexto" o pide que los tests reflejen la implementación real con base de datos.
---

# EF Repository Tests – Skill

Convierte tests de repositorios con almacenamiento en memoria a tests que usan **Entity Framework Core InMemory**, de forma que reflejen fielmente cómo funciona el repositorio real en producción.

---

## Cuándo aplica este skill

- El repositorio de producción recibe un `DbContext` por constructor (inyección de dependencias).
- Los tests actuales instancian el repositorio con `new Repo()` sin pasar contexto, o usan una implementación fake con lista interna.
- El usuario quiere que los tests usen la misma clase de repositorio real, con un contexto EF InMemory aislado por test.

---

## Patrón general de migración

### 1. Agregar el paquete NuGet de pruebas

El proyecto de tests necesita:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.*" />
```

Si el repositorio está en un proyecto separado y el `DbContext` vive ahí o en otro proyecto, asegurate de que el proyecto de tests tenga referencia a ese proyecto también.

### 2. Identificar el DbContext

Preguntale al usuario (o inferí del código dado) cuál es:
- El nombre del `DbContext` (ej: `WorldCupPlannerContext`)
- Qué `DbSet<T>` expone (ej: `DbSet<Team> Teams`)
- Si el contexto tiene constructor que acepta `DbContextOptions` (requerido para InMemory)

Si el contexto no tiene constructor con `DbContextOptions`, indicarle al usuario que debe agregarlo:

```csharp
public WorldCupPlannerContext(DbContextOptions<WorldCupPlannerContext> options)
    : base(options) { }
```

### 3. Estructura del test actualizado

Cada clase de test debe:

1. **Crear un contexto InMemory con nombre único por test** (para aislamiento total).
2. **Pasarlo al repositorio por constructor**.
3. **Limpiar / disponer el contexto en `[TestCleanup]`** (o usar `using`).

#### Plantilla base

```csharp
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Repository;
using YourNamespace.Data; // namespace del DbContext

namespace RepositoryTests;

[TestClass]
public class TeamRepositoryTests
{
    private WorldCupPlannerContext _context;
    private TeamRepository _teamRepository;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // único por test
            .Options;

        _context = new WorldCupPlannerContext(options);
        _teamRepository = new TeamRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // ... tests sin cambios en su lógica interna
}
```

> **Por qué `Guid.NewGuid()`**: InMemory databases con el mismo nombre comparten estado entre tests. Usar un GUID garantiza que cada test arranca con una base limpia sin necesidad de limpiar manualmente.

### 4. Ajustes en los tests individuales

En la mayoría de los casos los tests **no cambian su lógica interna**. Solo cambia el `Setup()`.

Sin embargo, hay casos que requieren atención:

#### Comparación de entidades (`Assert.AreEqual(team, result)`)

EF puede devolver una instancia diferente del mismo objeto (misma fila, objeto distinto en memoria). Si la entidad no sobreescribe `Equals`, las comparaciones por referencia fallarán.

**Opciones**:
- Comparar por propiedades específicas (`Assert.AreEqual(team.Name, result.Name)`)
- Implementar `Equals` / `GetHashCode` en la entidad
- Usar `Assert.IsTrue(teams.Any(t => t.Name == team1.Name))` en lugar de `Contains`

#### Test "GetAll devuelve copia, no la lista interna"

Con EF esto cambia semánticamente: `GetAll()` siempre devuelve una nueva lista (`.ToList()`), por lo que el test sigue siendo válido tal cual. No requiere cambios.

#### Tracking de EF y entidades detached

Si el repositorio hace `_context.Remove(existing); _context.Add(updated)` en el Update, puede haber conflictos de tracking. Si los tests fallan con errores de tracking, indicarle al usuario que puede usar `AsNoTracking()` en las queries del repositorio o llamar a `_context.ChangeTracker.Clear()` en el Cleanup.

---

## Ejemplo completo: TeamRepositoryTests migrado

```csharp
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Repository;

namespace RepositoryTests;

[TestClass]
public class TeamRepositoryTests
{
    private WorldCupPlannerContext _context;
    private TeamRepository _teamRepository;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WorldCupPlannerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldCupPlannerContext(options);
        _teamRepository = new TeamRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void AddTeam_ShouldStoreTeam()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var teams = _teamRepository.GetAll();

        Assert.AreEqual(1, teams.Count);
        Assert.AreEqual(team.Name, teams[0].Name); // comparar por valor, no referencia
    }

    [TestMethod]
    public void AddTeam_Null_ShouldThrowException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _teamRepository.Add(null));
    }

    [TestMethod]
    public void Get_ExistingTeam_ShouldReturnTeam()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var result = _teamRepository.Get("Uruguay");

        Assert.IsNotNull(result);
        Assert.AreEqual(team.Name, result.Name);
    }

    [TestMethod]
    public void Get_NonExistingTeam_ShouldReturnNull()
    {
        var result = _teamRepository.Get("Argentina");
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GivenMultipleTeams_WhenGetAll_ShouldReturnAllTeams()
    {
        var team1 = CreateTeam("Uruguay");
        var team2 = CreateTeam("Argentina");
        _teamRepository.Add(team1);
        _teamRepository.Add(team2);

        var teams = _teamRepository.GetAll();

        Assert.AreEqual(2, teams.Count);
        Assert.IsTrue(teams.Any(t => t.Name == "Uruguay"));
        Assert.IsTrue(teams.Any(t => t.Name == "Argentina"));
    }

    [TestMethod]
    public void DeleteTeam_WhenExists_ShouldRemoveTeam()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);
        _teamRepository.Delete(team);

        Assert.IsNull(_teamRepository.Get("Uruguay"));
    }

    [TestMethod]
    public void DeleteTeam_WhenNonExists_ShouldThrowException()
    {
        var team = CreateTeam("Uruguay");
        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Delete(team));
    }

    [TestMethod]
    public void UpdateTeam_WhenExists_ShouldUpdateTeam()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var updatedTeam = new Team(team.Name, new Confederation("CONMEBOL"), 1000);
        _teamRepository.Update(updatedTeam);

        var result = _teamRepository.Get(team.Name);

        Assert.IsNotNull(result);
        Assert.AreEqual("CONMEBOL", result.Confederation.Name);
        Assert.AreEqual(1000, result.FifaRanking);
    }

    [TestMethod]
    public void UpdateTeam_WhenNonExists_ShouldThrowException()
    {
        var team = CreateTeam("Uruguay");
        Assert.ThrowsException<InvalidOperationException>(() => _teamRepository.Update(team));
    }

    [TestMethod]
    public void GetAll_ShouldReturnCopy_NotInternalList()
    {
        var team = CreateTeam("Uruguay");
        _teamRepository.Add(team);

        var list = _teamRepository.GetAll();
        list.Clear();

        var result = _teamRepository.GetAll();

        Assert.AreEqual(1, result.Count);
    }

    private Team CreateTeam(string name)
    {
        return new Team(name, new Confederation("AFC"), 500);
    }
}
```

---

## Checklist de migración

- [ ] Paquete `Microsoft.EntityFrameworkCore.InMemory` agregado al proyecto de tests
- [ ] El `DbContext` tiene constructor que acepta `DbContextOptions`
- [ ] `[TestInitialize]` crea un contexto InMemory con GUID único
- [ ] El repositorio recibe el contexto por constructor
- [ ] `[TestCleanup]` llama a `EnsureDeleted()` y `Dispose()`
- [ ] Las comparaciones de entidades usan propiedades, no referencias de objeto
- [ ] Si hay errores de tracking, se revisa `AsNoTracking()` o `ChangeTracker.Clear()`

---

## Errores comunes y soluciones

| Error | Causa | Solución |
|---|---|---|
| `InvalidOperationException: Cannot track entity of type 'X'` | EF trackea la misma entidad dos veces | Usar `AsNoTracking()` en queries de lectura o `ChangeTracker.Clear()` |
| Tests se contaminan entre sí | Mismo nombre de DB InMemory compartido | Usar `Guid.NewGuid().ToString()` como nombre |
| `Assert.AreEqual` falla aunque los datos son iguales | Comparación por referencia en lugar de valor | Comparar propiedades o implementar `Equals` en la entidad |
| `NullReferenceException` en `_context` | Constructor del contexto no acepta `DbContextOptions` | Agregar constructor apropiado al `DbContext` |
