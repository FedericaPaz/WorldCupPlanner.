---
name: grasp-protected-variations
description: >
  Revisa el diseño aplicando el patrón Protected Variations. Identifica puntos 
  de inestabilidad y sugiere abstracciones o encapsulamiento para proteger 
  al sistema frente a cambios futuros sin introducir complejidad innecesaria.
---

# GRASP Protected Variations Skill

## Objetivo

Esta skill ayuda a revisar y mejorar el diseño del código aplicando el patrón GRASP Protected Variations.

El objetivo es identificar puntos del sistema que pueden cambiar y proteger al resto del código de esos cambios mediante interfaces, abstracciones o encapsulamiento.

## Cuándo usar esta skill

Usar esta skill cuando una parte del sistema pueda variar, por ejemplo:

- Motores de simulación
- Formatos de exportación
- Reglas de cálculo de ranking
- Acceso a datos
- Tipos de notificación
- Tipos de incidencias de partido
- Cambios futuros en reglas de negocio

## Criterio principal

Cuando una decisión de diseño puede cambiar en el futuro, el sistema debe aislar ese cambio para que no afecte a muchas clases.

En este proyecto, un ejemplo claro son los motores de simulación. El sistema depende de la interfaz `ISimulationEngine` y no directamente de una implementación concreta. Esto permite agregar nuevos motores sin modificar la lógica principal de simulación.

## Reglas de revisión

Al revisar código, verificar lo siguiente:

1. Identificar qué partes del sistema tienen mayor probabilidad de cambiar.
2. Verificar si esas partes están protegidas mediante interfaces o clases específicas.
3. Evitar que una regla variable esté duplicada en varias clases.
4. Evitar que cambios en una implementación concreta obliguen a modificar servicios, UI y repositorios al mismo tiempo.
5. Revisar si los servicios dependen de contratos y no de detalles internos.
6. Si una clase conoce demasiados detalles de otra, sugerir encapsular o abstraer.
7. Si agregar una nueva variante requiere modificar muchas clases, sugerir aplicar Protected Variations.
8. Mantener el cambio lo más localizado posible.

## Resultado esperado

Cuando se aplique esta skill, el agente debe responder con:

1. Punto de variación detectado.
2. Riesgo de cambio futuro.
3. Mecanismo de protección existente o sugerido.
4. Clases afectadas.
5. Refactor sugerido, si corresponde.
6. Código propuesto, solo si es necesario.

## Formato de respuesta esperado

Usar el siguiente formato:

### Punto de variación
Describir qué parte del sistema puede cambiar.

### Protección existente
Indicar si ya existe una interfaz, clase o mecanismo que aísla el cambio.

### Evaluación
Explicar si el diseño protege correctamente al resto del sistema.

### Observaciones
Listar problemas concretos si existen.

### Refactor sugerido
Indicar qué abstracción, encapsulamiento o separación se recomienda.

### Código sugerido
Incluir código solo si el cambio es necesario.

## Ejemplo de aplicación

Si el sistema permite elegir distintos motores de simulación, la lógica principal no debe depender de clases concretas como `ProbabilisticSimulationEngine`.

Debe depender de una abstracción como `ISimulationEngine`, permitiendo incorporar un nuevo motor sin modificar el flujo principal del sistema.

## Restricciones

- No agregar abstracciones innecesarias.
- No cambiar comportamiento funcional sin justificarlo.
- No duplicar reglas para proteger variaciones.
- Mantener compatibilidad con las pruebas existentes.
- Mantener la arquitectura por capas del proyecto.