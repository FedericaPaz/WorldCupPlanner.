---
name: grasp-high-cohesion
description: >
   Analiza la cohesión de clases, servicios y módulos según el principio GRASP High Cohesion e identifica responsabilidades excesivas o mezcladas. Usalo cuando quieras revisar diseño orientado a objetos, distribución de responsabilidades o clases con demasiados motivos de cambio.
---

# GRASP High Cohesion

## Cuando usar esta skill

Cuando el usuario pida:

* Revisar la aplicación del principio GRASP High Cohesion.
* Analizar responsabilidades de clases o servicios.
* Detectar clases con demasiadas funciones.
* Revisar distribución de responsabilidades.
* Identificar oportunidades para mejorar mantenibilidad.
* Auditar el diseño antes de una entrega o refactorización.
* Investigar problemas de acoplamiento originados por baja cohesión.

## Recordatorio del principio

GRASP High Cohesion busca asignar responsabilidades de forma que cada clase, componente o módulo tenga un propósito claro y bien definido.

Una clase con alta cohesión:

* Tiene responsabilidades relacionadas entre sí.
* Posee un objetivo claro dentro del sistema.
* Es fácil de comprender.
* Es fácil de mantener.
* Tiene pocos motivos de cambio.

Una clase con baja cohesión suele:

* Mezclar reglas de negocio diferentes.
* Realizar tareas no relacionadas.
* Acumular responsabilidades con el tiempo.
* Volverse difícil de mantener y extender.

No asumir que dividir una clase siempre es la solución correcta. Evaluar siempre el contexto funcional y arquitectónico.

## Proceso

1. Analizar el módulo, archivo o conjunto de archivos indicado por el usuario.

2. Identificar:

    * Clases.
    * Servicios.
    * Entidades.
    * Repositorios.
    * Generadores.
    * Helpers.
    * Componentes Razor.

3. Para cada clase analizar:

    * Responsabilidades principales.
    * Métodos públicos.
    * Métodos privados relevantes.
    * Dependencias utilizadas.
    * Datos que administra.

4. Evaluar si las responsabilidades:

    * Están relacionadas entre sí.
    * Persiguen un único objetivo general.
    * Corresponden al mismo concepto del dominio.
    * Podrían evolucionar juntas ante cambios del negocio.

5. Detectar señales de baja cohesión:

    * Métodos que trabajan sobre conceptos distintos.
    * Servicios que coordinan demasiados casos de uso.
    * Entidades que realizan tareas ajenas al dominio.
    * Clases con múltiples motivos de cambio.
    * Dependencias utilizadas únicamente por uno o dos métodos.
    * Código agrupado artificialmente por conveniencia.
    * Métodos que podrían pertenecer naturalmente a otra clase.

6. Analizar el impacto de la baja cohesión sobre:

    * Mantenibilidad.
    * Comprensión del código.
    * Acoplamiento.
    * Testabilidad.
    * Evolución futura del sistema.

7. Para cada problema encontrado:

    * Explicar por qué afecta la cohesión.
    * Identificar responsabilidades mezcladas.
    * Recomendar una mejora concreta.
    * Justificar la recomendación utilizando exclusivamente GRASP High Cohesion.

8. Presentar los hallazgos ordenados por impacto arquitectónico.

## Formato de salida

Para cada hallazgo utilizar el siguiente formato:

### Hallazgo

**Clase analizada:** [Nombre]

**Ubicación:** [Archivo o módulo]

**Nivel de cohesión:** Alto / Medio / Bajo

**Responsabilidades identificadas:**

* Responsabilidad 1
* Responsabilidad 2
* Responsabilidad 3

**Justificación:**
Explicar la evaluación utilizando GRASP High Cohesion.

**Posible mejora:**
Indicar qué responsabilidades podrían reorganizarse y por qué.

---

Finalizar siempre con:

### Resumen General

* Fortalezas detectadas.
* Problemas principales encontrados.
* Nivel general de cohesión del módulo analizado.
* Recomendaciones priorizadas.

## Restricciones

* No modificar código.
* No generar código automáticamente.
* No aplicar refactorizaciones.
* No crear Pull Requests.
* No realizar cambios en archivos.
* El objetivo principal es analizar y proponer mejoras.

Si el usuario solicita implementar una recomendación:

1. Explicar primero qué cambio se propone.
2. Justificarlo utilizando GRASP High Cohesion.
3. Esperar confirmación explícita antes de modificar código.
4. Preservar toda la lógica de negocio existente.
5. Mantener el comportamiento funcional actual.
6. No alterar reglas de negocio.
7. No romper tests existentes.
8. Realizar cambios mínimos y localizados.
9. Evitar mover responsabilidades sin justificación clara.
10. Ante cualquier duda sobre comportamiento funcional o reglas de negocio, solicitar confirmación antes de realizar cambios.

## Prioridad de análisis

Priorizar especialmente:

* Servicios de aplicación.
* Servicios de negocio.
* Entidades de dominio.
* Generadores de Fixture.
* Importadores.
* Managers.
* Coordinadores de casos de uso.
* Clases con gran cantidad de métodos.
* Clases con muchas dependencias inyectadas.

Prestar especial atención a clases que mezclen validaciones, reglas de negocio, persistencia, generación de datos, notificaciones o coordinación de procesos, ya que suelen ser candidatas a presentar baja cohesión.

La prioridad es detectar oportunidades de mejora de diseño sin comprometer la estabilidad funcional del sistema.
