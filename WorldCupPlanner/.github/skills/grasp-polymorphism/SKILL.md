---
name: grasp-polymorphism
description: >
  Revisa el diseño aplicando el patrón GRASP Polymorphism. Identifica lógica 
  basada en tipos o condicionales (if/switch) y propone su reemplazo por 
  invocación polimórfica para delegar el comportamiento a las clases específicas.
---

# GRASP Polymorphism Skill

## Objetivo

Esta skill ayuda a revisar y mejorar el diseño del código aplicando el patrón GRASP Polymorphism.

El objetivo es identificar casos donde el comportamiento cambia según un tipo, variante o estrategia, y evitar resolver esas diferencias con condicionales repetidos. En su lugar, se deben usar interfaces, clases concretas e invocación polimórfica.

## Cuándo usar esta skill

Usar esta skill cuando exista más de una forma de realizar una operación, por ejemplo:

- Motores de simulación
- Formatos de exportación
- Tipos de notificación
- Tipos de incidencias de partido
- Estrategias de cálculo
- Validaciones que cambian según el caso

## Criterio principal

Cuando el comportamiento depende de una variante, cada variante debe encapsular su propia lógica.

En este proyecto, un ejemplo claro son los motores de simulación:

- `ISimulationEngine`
- `ProbabilisticSimulationEngine`
- `PureRandomSimulationEngine`

El servicio que coordina la simulación no debe conocer los detalles internos de cada algoritmo. Solo debe depender de la interfaz y ejecutar el método definido por el contrato.

## Reglas de revisión

Al revisar código, verificar lo siguiente:

1. Evitar condicionales repetidos para decidir qué comportamiento ejecutar.
2. Identificar si existe una interfaz o clase abstracta que represente el contrato común.
3. Verificar que cada implementación concreta tenga una responsabilidad clara.
4. Confirmar que el cliente dependa de la abstracción y no de una clase concreta.
5. Evitar que un servicio conozca detalles internos de cada variante.
6. Si aparece un `if`, `switch` o comparación de strings para elegir comportamiento en muchos lugares, sugerir polimorfismo.
7. Si agregar una nueva variante obliga a modificar muchas clases, sugerir una abstracción.
8. Mantener nombres claros para interfaces e implementaciones.

## Resultado esperado

Cuando se aplique esta skill, el agente debe responder con:

1. Variante o comportamiento variable detectado.
2. Abstracción existente o sugerida.
3. Implementaciones concretas.
4. Evaluación de si se cumple GRASP Polymorphism.
5. Refactor sugerido, si corresponde.
6. Código propuesto, solo si es necesario.

## Formato de respuesta esperado

Usar el siguiente formato:

### Comportamiento variable
Describir qué operación cambia según el tipo o variante.

### Abstracción identificada
Indicar la interfaz o clase base utilizada.

### Implementaciones
Listar las clases concretas.

### Evaluación
Explicar si el diseño cumple o no con GRASP Polymorphism.

### Observaciones
Listar problemas concretos si existen.

### Refactor sugerido
Indicar qué condicionales, clases o dependencias habría que modificar.

### Código sugerido
Incluir código solo si el cambio es necesario.

## Ejemplo de aplicación

Si el sistema permite elegir entre un motor probabilístico y uno aleatorio puro, no se debe colocar toda la lógica en un único método con condicionales.

En su lugar, se debe definir una interfaz común, por ejemplo `ISimulationEngine`, y una implementación concreta por cada algoritmo. El servicio de simulación debe recibir la interfaz y ejecutar el método correspondiente sin conocer los detalles internos de cada motor.

## Restricciones

- No modificar comportamiento funcional sin justificarlo.
- No reemplazar polimorfismo existente por condicionales.
- No crear interfaces innecesarias si solo existe una implementación y no hay variabilidad real.
- Mantener compatibilidad con las pruebas existentes.
- Mantener la arquitectura por capas del proyecto.