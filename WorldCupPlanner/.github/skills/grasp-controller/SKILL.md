---
name: grasp-controller
description: >
  Revisa el diseño aplicando el patrón GRASP Controller. Identifica la lógica 
  de coordinación de casos de uso y asegura que esté correctamente centralizada 
  en servicios, evitando que la capa UI o los repositorios contengan lógica de negocio.
---

# GRASP Controller Skill

## Objetivo

Esta skill ayuda a revisar y mejorar el diseño del código aplicando el patrón GRASP Controller.

El objetivo es verificar que la coordinación de los casos de uso esté ubicada en clases controladoras de aplicación, como servicios, y no en la interfaz de usuario, entidades del dominio o repositorios.

## Cuándo usar esta skill

Usar esta skill cuando se implemente, revise o refactorice un caso de uso del sistema, por ejemplo:

- Crear usuario
- Editar usuario
- Generar fixture
- Simular partidos
- Cargar resultado
- Generar cruces
- Exportar información
- Marcar notificaciones como leídas

## Criterio principal

Para cada caso de uso, debe existir una clase que actúe como controlador de la operación.

En este proyecto, normalmente ese rol lo cumplen clases de la capa `Services`, por ejemplo:

- `UserService`
- `AuthService`
- `FixtureService`
- `MatchService`
- `CruceService`
- `NotificationService`
- `ExportService`

Estas clases deben recibir la solicitud desde la interfaz, validar el estado necesario, coordinar entidades y repositorios, y delegar responsabilidades específicas a otras clases.

## Reglas de revisión

Al revisar código, verificar lo siguiente:

1. La interfaz de usuario no debe contener lógica de negocio.
2. Las páginas `.razor` deben limitarse a capturar datos, invocar servicios y mostrar resultados.
3. Los repositorios no deben contener reglas de negocio; solo acceso a datos.
4. Las entidades del dominio deben contener reglas propias del dominio, pero no coordinar casos de uso completos.
5. Los servicios deben coordinar el flujo principal del caso de uso.
6. Si una pantalla llama directamente a varios repositorios o modifica demasiadas entidades, sugerir mover esa lógica a un servicio.
7. Si un servicio tiene demasiadas responsabilidades no relacionadas, sugerir dividirlo por caso de uso.
8. Si un método de servicio solo delega sin aportar coordinación, revisar si realmente es necesario.

## Resultado esperado

Cuando se aplique esta skill, el agente debe responder con:

1. Caso de uso analizado.
2. Clase que actúa como Controller.
3. Responsabilidades que cumple.
4. Problemas detectados, si existen.
5. Refactor sugerido, si corresponde.
6. Código propuesto, solo si es necesario.

## Formato de respuesta esperado

Usar el siguiente formato:

### Caso de uso revisado
Nombre del caso de uso.

### Controller identificado
Clase que coordina la operación.

### Evaluación
Explicar brevemente si cumple o no cumple con GRASP Controller.

### Observaciones
Listar problemas concretos si existen.

### Refactor sugerido
Indicar qué mover, crear o simplificar.

### Código sugerido
Incluir código solo si el cambio es necesario.

## Ejemplo de aplicación

Si una página Blazor crea un usuario y además valida roles, revisa duplicados, registra logs y accede directamente al repositorio, se debe sugerir mover esa coordinación a `UserService` o `AuthService`.

La página debe limitarse a construir el DTO, llamar al servicio correspondiente y mostrar el resultado al usuario.

## Restricciones

- No modificar comportamiento funcional sin justificarlo.
- No cambiar nombres públicos si no es necesario.
- No mover lógica de dominio a la interfaz.
- No mover acceso a datos a la interfaz.
- Mantener la arquitectura por capas del proyecto.
- Mantener compatibilidad con las pruebas existentes.