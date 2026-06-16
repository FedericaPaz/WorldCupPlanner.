# WorldCupPlanner

Aplicación educativa (entrega obligatoria) para planificar y simular un torneo tipo World Cup.

Descripción
-----------
WorldCupPlanner es una aplicación Blazor Server que permite gestionar equipos, partidos, estadios y usuarios; generar fixtures, simular resultados y calcular cruces de fases finales. Está diseñada para uso académico: la persistencia es en memoria (listas) en esta entrega.

Tecnologías
----------
- C# y .NET 8 (TargetFramework: net8.0)
- Blazor Server (Microsoft.NET.Sdk.Web)
- MSTest para pruebas unitarias
- Paquetes usados: Microsoft.EntityFrameworkCore (referenciado), Microsoft.EntityFrameworkCore.SqlServer (referenciado, no usado como BD en esta entrega), Blazored.LocalStorage, ClosedXML

Estructura del repositorio
-------------------------
La solución contiene varios proyectos organizados por responsabilidad:

- WorldCupPlanner.sln — solución principal
- Domain/ — entidades y lógica del dominio (entidades, validaciones)
- Services/ — casos de uso y lógica de aplicación (simulación, fixture, import, export)
- Repository/ — implementación de persistencia mediante repositorios y Entity Framework Core
- IRepository/ y IServices/ — contratos/interfaces
- DTOs/ — objetos de transferencia
- Web/ — proyecto Blazor Server (UI, componentes Razor)
- DomainTests/, ServicesTests/, RepositoryTests/ — proyectos de pruebas (MSTest)

Instrucciones para ejecutar
---------------------------
Requisitos previos:
- .NET 8 SDK instalado: https://dotnet.microsoft.com
- (Opcional) Visual Studio 2022/2023 o VS Code

Ejecutar desde la raíz del repositorio:

1. Entrar al directorio de la solución:
   cd WorldCupPlanner

2. Restaurar dependencias y compilar:
   dotnet restore
   dotnet build

3. Ejecutar la aplicación web (desde la carpeta WorldCupPlanner o indicando el proyecto):
   dotnet run --project Web\Web.csproj

4. Abrir el navegador en la URL que muestre la consola (por ejemplo https://localhost:5001 o http://localhost:5175). Seguir la salida del comando para el endpoint exacto.

Ejecutar tests:

- Desde la carpeta de la solución:
  dotnet test WorldCupPlanner.sln

Notas
-----
- Está diseñada para uso académico y utiliza una arquitectura por capas, con repositorios y Entity Framework Core para la persistencia.
- La aplicación utiliza Entity Framework Core para la persistencia.
- La configuración de conexión se encuentra en el proyecto Web.
- Los archivos exportados por la aplicación se guardan en `Web/wwwroot/exports/`.

Contacto
--------
Proyecto desarrollado como parte del obligatorio de Diseño de Aplicaciones 1.
