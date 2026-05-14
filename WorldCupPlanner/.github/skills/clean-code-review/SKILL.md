---
name: clean-code-review
description: Revisa únicamente problemas de Clean Code en código C#. Detecta nombres poco claros, atributos mal definidos, funciones largas, clases con demasiadas responsabilidades, comentarios innecesarios, duplicación, formato y manejo de errores. No modifica archivos ni implementa cambios.
---

# Clean Code Review

## Objetivo

Esta skill sirve solo para revisar Clean Code en archivos C# del proyecto.

Debe analizar el código y devolver observaciones concretas, sin modificar archivos, sin crear código nuevo completo y sin cambiar el comportamiento del sistema.

---

## Cuándo usar esta skill

Usar esta skill cuando quieras:

- Revisar si un archivo cumple con Clean Code.
- Detectar nombres poco descriptivos.
- Detectar atributos mal declarados.
- Verificar que los atributos privados empiecen con `_`.
- Verificar que las propiedades estén separadas de los atributos.
- Detectar funciones demasiado largas.
- Detectar funciones con más de una responsabilidad.
- Detectar clases con demasiadas responsabilidades.
- Detectar duplicación de código.
- Revisar comentarios innecesarios.
- Revisar formato e indentación.
- Revisar manejo de errores.

---

# Reglas de Clean Code a revisar

## 1. Atributos y propiedades en C#

### Reglas obligatorias del proyecto

- Todos los atributos deben ser `private`.
- Todos los atributos privados deben comenzar con `_`.
- No deben existir atributos públicos.
- No deben existir atributos sin modificador explícito.
- Si un dato debe exponerse, debe hacerse con una propiedad pública separada.
- Las propiedades públicas deben usar PascalCase.
- Los atributos privados deben usar camelCase con `_`.
- No sugerir auto-properties si el proyecto usa atributo privado + propiedad explícita.

### Ejemplo correcto

```csharp
private string _name;
private DateTime _birthDate;

public string Name
{
    get { return _name; }
    set { _name = value; }
}

public DateTime BirthDate
{
    get { return _birthDate; }
    set { _birthDate = value; }
}
```

### Ejemplo incorrecto

```csharp
public string name;
private string Name;
string email;
public string Name { get; set; }
```

---

## 2. Nombres significativos

Revisar que los nombres revelen intención.

Un buen nombre debe dejar claro:

- Por qué existe.
- Qué hace.
- Cómo se usa.

### Revisar

- Nombres de clases.
- Nombres de métodos.
- Nombres de atributos.
- Nombres de propiedades.
- Nombres de variables.
- Nombres de parámetros.

### Reglas

- Las clases deben tener nombres con sustantivos o frases nominales.
- Los métodos deben tener nombres con verbos o frases verbales.
- Los nombres deben ser pronunciables.
- Los nombres deben ser fáciles de buscar.
- Usar una sola palabra por concepto.
- No usar la misma palabra para conceptos distintos.

### Evitar

- `a`, `b`, `x`, `aux`, `tmp`, `obj`, `data`, `info`.
- Abreviaturas innecesarias.
- Nombres que no expliquen intención.
- Nombres demasiado parecidos.
- Slang o coloquialismos.
- Contexto innecesario.

---

## 3. Funciones y métodos

Revisar que las funciones sean simples y claras.

### Reglas

- Deben ser pequeñas.
- Deben hacer una sola cosa.
- Deben tener un único nivel de abstracción.
- Deben tener nombres descriptivos.
- Deben leerse de arriba hacia abajo.
- Deben evitar demasiada indentación.

### Parámetros

- Ideal: 0 parámetros.
- Aceptable: 1 parámetro.
- Evitar muchos parámetros.
- Evitar parámetros booleanos.

### Detectar

- Métodos largos.
- Métodos que hacen muchas cosas.
- Métodos con muchos `if`, `else`, `for` o `while` anidados.
- Métodos con parámetros booleanos.
- Métodos que mezclan validación, lógica y otras responsabilidades.

---

## 4. Comentarios

Revisar si los comentarios son necesarios.

### Reglas

- Preferir código autoexplicativo antes que comentarios.
- Evitar comentarios redundantes.
- Evitar comentarios obsoletos.
- Evitar comentarios que explican código confuso.

Si un comentario existe porque el código no se entiende, recomendar mejorar nombres o extraer métodos.

---

## 5. Formato

Revisar que el código sea legible.

### Reglas

- Mantener indentación consistente.
- Separar ideas con líneas en blanco.
- No alinear código horizontalmente de forma innecesaria.
- Mantener juntas las funciones relacionadas.
- Si un método A llama a un método B, B debería estar debajo de A cuando sea posible.
- El archivo no debería ser excesivamente largo.

---

## 6. Clases

Revisar que las clases sean simples y cohesivas.

### Reglas

- Una clase debe ser pequeña.
- Una clase debe tener una sola responsabilidad.
- Una clase debe tener una sola razón para cambiar.
- Los métodos y atributos de la clase deben estar relacionados entre sí.

### Detectar

- Clases que hacen demasiadas cosas.
- Clases con baja cohesión.
- Clases con muchos métodos no relacionados.
- Clases que mezclan responsabilidades.

---

## 7. Objetos y estructuras de datos

Revisar que no se mezclen responsabilidades.

### Reglas

- Los objetos deben ocultar datos y exponer comportamiento.
- Los DTOs deben transportar datos y no tener lógica importante.
- Evitar objetos híbridos.
- Respetar la Ley de Demeter.

### Detectar cadenas como

```csharp
obj.GetA().GetB().DoSomething();
```

Recomendar algo más directo cuando corresponda.

---

## 8. Manejo de errores

Revisar que los errores sean claros.

### Reglas

- Usar excepciones en lugar de códigos de error.
- No mezclar lógica principal con manejo de errores complejo.
- Evitar retornar `null` cuando pueda generar errores posteriores.
- Las excepciones deben tener mensajes claros.
- Los mensajes de error deben dar contexto.

---

## 9. Duplicación

Revisar código repetido.

### Detectar

- Bloques repetidos.
- Validaciones repetidas.
- Condiciones repetidas.
- Métodos muy parecidos.
- Nombres distintos para la misma idea.

Recomendar extraer métodos solo cuando mejore la claridad.

---

# Proceso que debe seguir la skill

1. Leer únicamente el archivo o carpeta indicada por el usuario.
2. Revisar solo aspectos de Clean Code.
3. No modificar archivos.
4. No crear archivos.
5. No implementar refactorizaciones.
6. No cambiar lógica de negocio.
7. No cambiar tests.
8. No ejecutar cambios automáticos.
9. Listar los problemas encontrados.
10. Explicar por qué cada problema afecta la legibilidad o mantenibilidad.
11. Dar una sugerencia concreta para corregir cada problema.
12. Mostrar ejemplos cortos solo si ayudan a entender la corrección.

---

# Formato de respuesta obligatorio

## Resumen

- Puntaje estimado de Clean Code: X/10
- Cantidad de problemas encontrados:
- Problemas más importantes:

## Hallazgos

### [Crítico / Importante / Mejora] Nombre del problema

- Ubicación:
- Problema:
- Regla de Clean Code afectada:
- Por qué importa:
- Sugerencia:

## Prioridad de corrección

1. ...
2. ...
3. ...

## Cosas que están bien

- ...

---

# Restricciones importantes

- No modificar código.
- No crear archivos.
- No reescribir clases completas.
- No implementar tests.
- No analizar arquitectura general del sistema salvo que afecte directamente Clean Code del archivo revisado.
- No proponer cambios grandes si no son necesarios.
- No cambiar comportamiento funcional.
- No sugerir auto-properties si el proyecto usa atributos privados con `_` y propiedades explícitas.
- No revisar cobertura de tests, salvo que el usuario lo pida explícitamente.
- No ejecutar comandos salvo que el usuario lo pida explícitamente.

---

# Ejemplos de uso

- `/clean-code-review revisar Services/UserService.cs`
- `/clean-code-review revisar Domain/User.cs`
- `/clean-code-review revisar Repository/UserRepository.cs`
- `/clean-code-review revisar Web/Pages/Users/UserEdit.razor`
- `/clean-code-review revisar ServicesTests/UserServiceTests.cs`