---
name: solid-ocp-domain
description: Analiza la aplicación del principio Open/Closed Principle (OCP) en las entidades del dominio.
---
# SOLID - Open/Closed Principle (OCP) en el Dominio

## Cuando usar esta skill

Cuando el usuario quiera:

* Revisar la aplicación de OCP en entidades del dominio.
* Identificar oportunidades de extensión mediante polimorfismo.
* Detectar comportamientos rígidos dentro del dominio.
* Analizar entidades que podrían requerir modificaciones ante nuevos requerimientos.
* Preparar una entrega de Diseño.
* Cumplir el requisito de aplicar OCP en al menos una clase del dominio.

## Objetivo

Evaluar exclusivamente la aplicación del principio:

* Open/Closed Principle (OCP)

El análisis debe enfocarse en las entidades del dominio y determinar si permiten incorporar nuevos comportamientos mediante extensión en lugar de modificación.

El objetivo principal es identificar al menos una oportunidad válida de aplicación de OCP dentro del dominio.

No evaluar otros principios SOLID salvo que sean estrictamente necesarios para justificar un hallazgo.

## Recordatorio del principio

Una clase cumple OCP cuando:

* Está abierta a la extensión.
* Está cerrada a la modificación.

Esto significa que nuevos comportamientos deberían incorporarse agregando nuevas implementaciones o extensiones, evitando modificar código existente y estable.

Una violación de OCP suele manifestarse cuando:

* Agregar una nueva variante obliga a modificar código existente.
* Aparecen nuevos if o switch.
* El comportamiento depende de enums o strings.
* Existe lógica que crece cada vez que surge un nuevo caso.

No recomendar abstracciones innecesarias ni sobreingeniería.

## Proceso

### Paso 1 - Identificar entidades del dominio

Analizar prioritariamente:

* Entidades.
* Value Objects.
* Agregados.
* Jerarquías de dominio.
* Reglas de negocio implementadas dentro del dominio.

Ignorar capas de infraestructura salvo que sean necesarias para comprender el contexto.

### Paso 2 - Identificar puntos de variación

Buscar comportamientos que probablemente evolucionen:

* Tipos de partidos.
* Tipos de cruces.
* Tipos de simulación.
* Estrategias de clasificación.
* Reglas de cálculo.
* Validaciones variables.
* Procesos con múltiples variantes.

Registrar posibles escenarios futuros de extensión.

### Paso 3 - Detectar señales de incumplimiento

Buscar especialmente:

* Switch sobre tipos de dominio.
* If encadenados.
* Comparaciones por enum.
* Comparaciones por string.
* Métodos que agregan nuevos casos frecuentemente.
* Comportamientos centralizados que crecerían ante nuevas variantes.

Evaluar si una nueva funcionalidad obligaría a modificar una entidad existente.

### Paso 4 - Evaluar impacto

Determinar:

* Riesgo de modificaciones futuras.
* Impacto sobre mantenibilidad.
* Riesgo de regresiones.
* Dificultad para agregar nuevas variantes.
* Impacto sobre pruebas existentes.

### Paso 5 - Proponer soluciones

Para cada hallazgo:

* Explicar claramente el problema.
* Describir qué requerimiento futuro obligaría a modificar la entidad.
* Recomendar una solución compatible con OCP.
* Explicar cómo la solución permitiría extender comportamiento sin modificar código estable.

No implementar cambios.

## Formato de salida

### Hallazgo

**Título:** [Nombre breve]

**Prioridad:** Alta / Media / Baja

**Entidad involucrada:** [Nombre]

**Ubicación:** [Archivo o módulo]

**Cumple requisito de OCP en Dominio:**
Sí / No

**Nivel de cumplimiento OCP:**
Alto / Medio / Bajo

**Descripción del problema:**

Explicar claramente por qué la entidad no está preparada para incorporar nuevas variantes sin modificación.

**Escenario de extensión futura:**

Describir un requerimiento futuro razonable que obligaría a modificar la clase actual.

**Por qué es un problema:**

Generar un párrafo explicando:

* Qué riesgo genera actualmente.
* Qué modificaciones futuras serían necesarias.
* Cómo afecta mantenibilidad y evolución del dominio.
* Cómo aumenta el riesgo de errores sobre código estable.

**Solución recomendada:**

Describir la mejora propuesta.

No generar código.

**Beneficios esperados:**

Generar un párrafo explicando:

* Cómo permite extender comportamiento sin modificar la entidad actual.
* Cómo reduce el riesgo de regresiones.
* Cómo mejora la mantenibilidad.
* Cómo facilita futuras extensiones del dominio.

---

### Resumen General

#### Fortalezas

Aspectos positivos encontrados.

#### Problemas Prioritarios

Ordenados por impacto.

#### Mejor candidato para aplicar OCP

Identificar la entidad más adecuada para implementar OCP.

Justificar la elección.

#### Nivel General de Cumplimiento OCP en el Dominio

Alto / Medio / Bajo

#### Recomendaciones Priorizadas

1. ...
2. ...
3. ...

#### Recomendación para la Entrega

Indicar cuál de los hallazgos resulta más defendible académicamente para demostrar la aplicación de OCP en una entidad del dominio.

## Restricciones

* No modificar código.
* No generar código automáticamente.
* No aplicar refactorizaciones.
* No crear Pull Requests.
* No realizar cambios en archivos.
* No cambiar comportamiento funcional.
* No alterar reglas de negocio.
* No introducir abstracciones innecesarias.
* No proponer soluciones que agreguen complejidad injustificada.
* El objetivo principal es analizar y recomendar mejoras.

Si el usuario solicita implementar una recomendación:

1. Explicar previamente el cambio.
2. Justificarlo utilizando únicamente OCP.
3. Esperar confirmación explícita.
4. Mantener exactamente el comportamiento funcional actual.
5. Mantener compatibilidad con los tests existentes.
6. Realizar cambios mínimos y localizados.
7. Preservar todas las reglas de negocio existentes.
8. Solicitar confirmación ante cualquier duda funcional.

## Prioridad de análisis

Priorizar especialmente:

* Match
* Team
* Group
* Stadium
* Cruce
* User

Buscar especialmente:

* If o switch por tipo.
* Comparaciones por enum.
* Comparaciones por string.
* Reglas de negocio que podrían requerir nuevas variantes.
* Comportamientos que crecerían con nuevos requerimientos.

La prioridad es identificar oportunidades reales de aplicar OCP en el dominio sin comprometer la estabilidad funcional ni modificar el comportamiento actual del sistema.
