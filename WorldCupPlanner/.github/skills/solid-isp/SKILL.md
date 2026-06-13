# SOLID — Interface Segregation Principle (ISP)

## Qué es

El principio de segregación de interfaces indica que una clase no debería depender de métodos que no utiliza.

En términos simples: es preferible tener interfaces chicas y específicas antes que una interfaz grande con muchas responsabilidades distintas.

---

## Señales de violación

- Una interfaz tiene métodos que no todas sus implementaciones necesitan.
- Una clase implementa métodos vacíos o con `NotImplementedException`.
- Un cliente depende de una interfaz grande aunque solo use una parte.
- Una interfaz mezcla responsabilidades no relacionadas.
- Cambiar una operación obliga a modificar clases que no tienen relación con ese cambio.

---

## Cómo identificarlo en C#

```csharp
// VIOLA ISP — una interfaz demasiado grande
public interface ITournamentService
{
    void CreateUser();
    void GenerateFixture();
    void ExportLogs();
    void SendNotification();
}

// Una clase queda obligada a depender de métodos que no usa.
```

```csharp
// CORRECTO — interfaces separadas por responsabilidad
public interface IUserService
{
    void CreateUser();
}

public interface IFixtureService
{
    void GenerateFixture();
}

public interface IExportService
{
    void ExportLogs();
}

public interface INotificationService
{
    void SendNotification();
}
```

---

## Casos en este proyecto

### Interfaces de servicios

El sistema separa contratos por responsabilidad, por ejemplo:

- `IAuthService`
- `IMatchService`
- `ICruceService`
- `INotificationService`
- `IExportService`
- `ISimulationService`

Esto evita que una clase o pantalla dependa de operaciones que no necesita.

### Motores de simulación

`ISimulationEngine` define únicamente la operación necesaria para simular un partido. Sus implementaciones no están obligadas a implementar métodos ajenos a esa responsabilidad.

Esto permite que `ProbabilisticSimulationEngine` y `PureRandomSimulationEngine` compartan un contrato simple y específico.

---

## Regla práctica

Si una interfaz empieza a crecer con métodos de diferentes casos de uso, conviene dividirla.

Una buena interfaz debería poder explicarse con una sola responsabilidad clara.

---

## Diferencia con SRP

- **SRP** se enfoca en que una clase tenga una sola razón para cambiar.
- **ISP** se enfoca en que una interfaz no obligue a depender de métodos innecesarios.

Una clase puede cumplir SRP pero depender de una interfaz demasiado grande. En ese caso, el problema es de ISP.

---

## Señal de buen diseño

Una interfaz cumple ISP cuando sus clientes pueden usarla sin conocer ni depender de operaciones que no forman parte de su necesidad concreta.