---

name: grasp-creator
description: Analiza la creación de objetos según el principio GRASP Creator e identifica responsabilidades de creación mal asignadas. Usalo cuando quieras revisar diseño orientado a objetos, uso de new, Factories o Generators.
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

# GRASP Creator

## Cuando usar esta skill

Cuando el usuario pida:

* Revisar la aplicación del principio GRASP Creator.
* Analizar responsabilidades de creación de objetos.
* Revisar el uso de new en el sistema.
* Evaluar Factories, Builders o Generators.
* Detectar problemas de cohesión o acoplamiento relacionados con la creación de objetos.
* Auditar el diseño antes de una entrega o refactorización.
* Buscar oportunidades de mejora en la asignación de responsabilidades.

## Recordatorio del principio

GRASP Creator establece que una clase debería crear objetos cuando:

* Los contiene o agrega.
* Los registra.
* Posee la información necesaria para inicializarlos.
* Es experta en la información requerida para su creación.

No asumir que toda creación debe moverse a una Factory. Evaluar siempre el contexto y las responsabilidades existentes.

## Proceso

1. Analizar el módulo, archivo o conjunto de archivos indicado por el usuario.

2. Identificar todas las instanciaciones relevantes realizadas mediante:

    * new
    * Factories
    * Builders
    * Generators
    * Métodos de creación especializados

3. Para cada creación encontrada registrar:

    * Clase creadora.
    * Objeto creado.
    * Método donde ocurre la creación.

4. Evaluar si la clase creadora:

    * Contiene el objeto creado.
    * Lo agrega o administra.
    * Posee la información necesaria para construirlo.
    * Es experta en la información requerida.
    * Mantiene una responsabilidad coherente con el resto de la clase.

5. Analizar el impacto de la decisión sobre:

    * Cohesión.
    * Acoplamiento.
    * Encapsulamiento.
    * Claridad del modelo de dominio.

6. Detectar posibles problemas:

    * Servicios que crean demasiados tipos de objetos.
    * Componentes Razor o UI que crean entidades de dominio.
    * Repositorios que crean objetos ajenos a persistencia.
    * Clases con responsabilidades de creación innecesarias.
    * Uso excesivo de new que aumente el acoplamiento.
    * Factories o Generators innecesarios.

7. Para cada problema encontrado:

    * Explicar por qué incumple o aplica parcialmente GRASP Creator.
    * Identificar riesgos de diseño.
    * Recomendar una mejora concreta.
    * Justificar la recomendación utilizando exclusivamente GRASP Creator.

8. Presentar los hallazgos ordenados por impacto arquitectónico.

## Formato de salida

Para cada hallazgo utilizar el siguiente formato:

### Hallazgo

**Objeto creado:** [Nombre]

**Clase responsable actual:** [Nombre]

**Ubicación:** [Archivo o método]

**Cumple Creator:** Sí / Parcial / No

**Justificación:**
Explicar la evaluación utilizando GRASP Creator.

**Posible mejora:**
Indicar qué clase sería una mejor candidata para asumir la responsabilidad y por qué.

---

Finalizar siempre con:

### Resumen General

* Fortalezas detectadas.
* Problemas principales encontrados.
* Nivel general de cumplimiento de GRASP Creator.
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
2. Justificarlo utilizando GRASP Creator.
3. Esperar confirmación explícita antes de modificar código.
4. Preservar toda la lógica de negocio existente.
5. Mantener el comportamiento funcional actual.
6. No alterar reglas de negocio.
7. No romper tests existentes.
8. Realizar cambios mínimos y localizados.
9. Evitar introducir nuevas dependencias innecesarias.
10. Ante cualquier duda sobre comportamiento funcional o reglas de negocio, solicitar confirmación antes de realizar cambios.

## Prioridad de análisis

Priorizar especialmente:

* Entidades de Dominio.
* Servicios.
* Generadores de Fixture.
* Importadores.
* Factories.
* Componentes Web o Razor.
* Casos donde existan múltiples responsabilidades de creación.

La prioridad es detectar oportunidades de mejora de diseño sin comprometer la estabilidad funcional del sistema.
