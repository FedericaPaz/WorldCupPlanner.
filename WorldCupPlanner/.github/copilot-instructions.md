# Proyecto

WorldCupPlanner 2026.

Aplicación desarrollada para el obligatorio de Diseño de Aplicaciones 1.  
El sistema permite gestionar usuarios, equipos, estadios, partidos, generación de fixture, simulación de resultados, cruces de segunda fase, importación de equipos y auditoría.

En esta primera entrega la información se maneja en memoria. No usar base de datos.

# Stack

- C#
- .NET 8
- Blazor Server App
- MSTest
- GitHub Actions

# Estructura

La estructura esperada del proyecto es la siguiente:

- `Dominio/` — entidades, clases del dominio, excepciones y reglas de negocio
- `Servicios/` — lógica de aplicación y casos de uso, si corresponde
- `Memoria/` o `Repositorios/` — persistencia en memoria e interfaces de acceso a datos
- `Web/` — páginas y componentes Razor de Blazor Server
- `Tests/` — tests unitarios con MSTest

Si los nombres reales de carpetas cambian, usar siempre la estructura existente del repositorio como referencia principal.

# Arquitectura

## Clases de dominio esperadas
- `Partido`
- `Usuario`
- `Equipo`
- `Estadio`
- `Confederacion`
- `Rol`

También pueden existir otras clases del dominio si el diseño del proyecto lo requiere, por ejemplo para auditoría, configuración o agrupación de entidades.

## Persistencia
En esta primera entrega los datos se guardan en memoria, por ejemplo usando `List<>`.

Puede existir una capa de repositorio o memoria según el diseño del proyecto.

## UI
La capa `Web` contiene páginas y componentes Razor.
No debe contener lógica de negocio.

# Convenciones de arquitectura

- La UI no debe contener lógica de negocio.
- `Web` no debe acceder directamente a persistencia.
- Mantener separación clara entre interfaz, lógica de aplicación y dominio.
- No asumir UML disponible como fuente de verdad.
- Si falta contexto de diseño, inferirlo a partir del código existente y hacer cambios conservadores.
- No inventar clases, capas o servicios innecesarios si todavía no están definidos en el proyecto.

# Convenciones de código

- Seguir principios de Clean Code.
- Priorizar clases con una sola responsabilidad.
- Usar nombres claros y específicos.
- Evitar duplicación.
- Mantener métodos cortos y legibles.
- No introducir cambios fuera del alcance pedido.
- No hacer refactors grandes si no fueron solicitados.
- Mantener consistencia con la arquitectura ya definida en el proyecto.

# TDD y testing

- El proyecto debe desarrollarse siguiendo TDD.
- Escribir primero el test y luego el código mínimo necesario para hacerlo pasar.
- Refactorizar solo después de tener los tests en verde.
- Usar MSTest.
- Toda lógica nueva de negocio debe venir acompañada de tests.
- No romper tests existentes.
- Los tests deben ser claros, independientes y reproducibles.
- Priorizar casos válidos, inválidos y bordes.
- Si se modifica comportamiento existente, revisar si corresponde actualizar tests.
- Ante una tarea nueva, priorizar siempre una solución compatible con TDD antes que una implementación rápida sin cobertura.

# Restricciones

- No agregar base de datos en esta entrega.
- No cambiar el framework de testing.
- No agregar dependencias externas sin necesidad clara.
- No inventar reglas de negocio que no estén en la consigna, en el código existente o en instrucciones explícitas del equipo.
- No mover lógica a la UI para resolver rápido un requerimiento.
- No agregar capas o complejidad innecesaria.