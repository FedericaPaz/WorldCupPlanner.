---
name: grasp-high-cohesion
description: Analiza la aplicación del principio GRASP Alta Cohesión. Identifica clases, servicios y componentes con responsabilidades excesivas o poco relacionadas, proponiendo mejoras sin modificar el comportamiento existente.
---
# GRASP - Alta Cohesión

## Cuando usar esta skill

Cuando el usuario quiera:

* Revisar la cohesión de una clase o módulo.
* Detectar servicios con demasiadas responsabilidades.
* Analizar la distribución de responsabilidades.
* Identificar clases con múltiples motivos de cambio.
* Evaluar la mantenibilidad de una solución.
* Preparar una entrega de Diseño.
* Revisar observaciones relacionadas con cohesión.

## Objetivo

Evaluar exclusivamente la aplicación del principio:

* GRASP Alta Cohesión.

El análisis debe centrarse en las responsabilidades de cada clase y en qué tan relacionadas están entre sí.

No evaluar otros principios GRASP ni principios SOLID salvo que sean estrictamente necesarios para justificar un hallazgo.

## Recordatorio del principio

Una clase presenta alta cohesión cuando:

* Sus responsabilidades están relacionadas entre sí.
* Tiene un propósito claro.
* Sus métodos colaboran para cumplir un mismo objetivo.
* Posee pocos motivos de cambio.

Una clase presenta baja cohesión cuando:

* Realiza demasiadas tareas distintas.
* Mezcla responsabilidades no relacionadas.
* Tiene múltiples motivos de cambio.
* Contiene métodos que parecen pertenecer a otras clases.
* Resulta difícil de comprender o mantener.

## Proceso

### Paso 1 - Identificar responsabilidades

Para cada clase analizar:

* Objetivo principal.
* Responsabilidades funcionales.
* Responsabilidades técnicas.
* Responsabilidades de coordinación.
* Responsabilidades de validación.
* Responsabilidades de persistencia.
* Responsabilidades de generación o cálculo.

Registrar todas las responsabilidades detectadas.

### Paso 2 - Analizar cohesión

Evaluar si:

* Las responsabilidades están relacionadas.
* Los métodos colaboran para cumplir un mismo propósito.
* Existe un objetivo claramente definido.
* La clase posee una única razón principal para cambiar.
* Los datos administrados están alineados con las responsabilidades.

### Paso 3 - Detectar señales de baja cohesión

Buscar especialmente:

* Servicios que mezclen múltiples procesos de negocio.
* Clases que coordinen tareas no relacionadas.
* Métodos que podrían pertenecer naturalmente a otra clase.
* Componentes que mezclen lógica funcional y técnica.
* Clases con responsabilidades crecientes.
* Clases con muchos motivos de cambio.
* Servicios demasiado grandes para un único propósito.

### Paso 4 - Evaluar impacto

Determinar:

* Impacto en mantenibilidad.
* Impacto en comprensión del código.
* Impacto en evolución futura.
* Impacto en reutilización.
* Riesgo de errores al modificar la clase.

### Paso 5 - Proponer soluciones

Para cada hallazgo:

* Explicar claramente el problema.
* Identificar las responsabilidades mezcladas.
* Recomendar una mejora concreta.
* Justificar cómo la mejora aumenta la cohesión.

No implementar cambios.

## Formato de salida

### Hallazgo

**Título:** [Nombre breve]

**Prioridad:** Alta / Media / Baja

**Ubicación:** [Clase o módulo]

**Responsabilidades identificadas:**

* ...
* ...
* ...

**Nivel de cohesión:**

Alta / Media / Baja

**Descripción del problema:**

Explicar qué responsabilidades están mezcladas o por qué la cohesión es insuficiente.

**Por qué es un problema:**

Generar un párrafo explicando:

* Qué riesgos genera actualmente.
* Cómo afecta la mantenibilidad.
* Cómo afecta la comprensión del sistema.
* Cómo afecta la evolución futura del código.

**Solución recomendada:**

Describir la mejora propuesta.

No generar código.

**Beneficios esperados:**

Generar un párrafo explicando:

* Cómo aumenta la cohesión.
* Cómo mejora la claridad de responsabilidades.
* Cómo facilita el mantenimiento.
* Cómo reduce la complejidad futura.

---

### Resumen General

#### Fortalezas

Aspectos positivos encontrados.

#### Problemas Prioritarios

Ordenados por impacto.

#### Nivel General de Cohesión

Alto / Medio / Bajo

#### Recomendaciones Priorizadas

1. ...
2. ...
3. ...

## Restricciones

* No modificar código.
* No generar código automáticamente.
* No aplicar refactorizaciones.
* No crear Pull Requests.
* No realizar cambios en archivos.
* No cambiar comportamiento funcional.
* No alterar reglas de negocio.
* El objetivo principal es analizar y recomendar mejoras.

Si el usuario solicita implementar una recomendación:

1. Explicar previamente el cambio.
2. Justificarlo utilizando únicamente GRASP Alta Cohesión.
3. Esperar confirmación explícita.
4. Mantener exactamente el comportamiento funcional actual.
5. Mantener compatibilidad con los tests existentes.
6. Realizar cambios mínimos y localizados.
7. Preservar todas las reglas de negocio existentes.
8. Solicitar confirmación ante cualquier duda funcional.

## Prioridad de análisis

Priorizar especialmente:

* Servicios con muchos métodos.
* Servicios con múltiples responsabilidades.
* Clases que coordinen varios procesos.
* Componentes con lógica funcional y técnica mezcladas.
* Clases con gran cantidad de código.
* Servicios que hayan crecido durante el desarrollo del proyecto.

La prioridad es detectar oportunidades de mejora en la distribución de responsabilidades sin comprometer la estabilidad funcional del sistema.
