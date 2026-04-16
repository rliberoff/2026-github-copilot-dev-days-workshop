# Ejercicio Greenfield: TodoLite CLI

**Duración estimada**: 45 minutos
**Objetivo**: Construir una aplicación desde cero usando el flujo completo de SDD con Spec Kit.

## Premisa del escenario

> Construye una CLI mínima de tareas llamada **TodoLite** que soporte los comandos `add`, `list`, `done` y `rm` con persistencia en un archivo JSON.

Los participantes generan todos los artefactos SDD en vivo durante la sesión. El código de referencia en el repositorio sirve como respaldo si el código generado por el agente diverge o el tiempo se agota.

## Requisitos previos

- Entorno verificado (ver [00-preparacion.md](00-preparacion.md))
- VS Code con GitHub Copilot activo
- Runtime del lenguaje elegido instalado (.NET 10 SDK o Python 3.11+)

## Paso a paso

### 1. Inicializar el proyecto

Crea un directorio nuevo para tu ejercicio y navega a él:

```bash
mkdir todolite
cd todolite
git init -b main
```

### 2. Inicializar Spec Kit

```bash
specify init --here --integration copilot --force
```

Según estés en Windows o MacOS/Linux, el asistente de configuración de Spec Kit te preguntará si los *scripts* deben ser PowerShell (`ps`) o Bash (`sh`). Elige la opción que corresponda a tu entorno.

Verifica que el directorio `.specify/` se haya creado con la constitución, plantillas y comandos registrados.

### 3. Crear la constitución

La constitución es el **documento de gobernanza de más alto nivel del proyecto**. Define los principios que todos los artefactos SDD posteriores (spec, plan, tasks y código) deben respetar. No es un simple listado de buenas prácticas: es la fuente de verdad que el agente consulta en cada paso del flujo para garantizar coherencia entre decisiones técnicas y restricciones del proyecto. Una constitución bien escrita reduce la ambigüedad, acelera las revisiones y hace que el código generado sea más predecible y alineado con las intenciones del equipo.

Como toda constitución, que representa en cierta forma a un marco legal, es suceptible de mejoras (enmiendas) a lo largo del tiempo. Sin embargo, es importante que los principios fundamentales estén claros desde el inicio para guiar la generación de artefactos y código de manera consistente. Entre ciclos de implementación, el equipo debe revisar y actualizar la constitución para reflejar aprendizajes, cambios en el alcance o nuevas decisiones técnicas. Esto asegura que la constitución siga siendo relevante y útil a medida que el proyecto evoluciona.

Abre el VS Code en el directorio del proyecto y ejecuta en el chat del agente:

```text
/speckit.constitution
```

Proporciona principios como:

- El código debe ser ejecutable y probado
- Usar persistencia JSON para almacenamiento
- Seguir convenciones del lenguaje elegido
- En escenarios más reales, toma en cuenta aspectos como seguridad, rendimiento, escalabilidad, experiencia de usuario, etc.

#### Cuándo usar `(NON-NEGOTIABLE)`

Puedes marcar un principio como `(NON-NEGOTIABLE)` cuando se trate de una regla tan fundamental que **ninguna excepción esté permitida bajo ninguna circunstancia**. Estos principios definen la identidad del proyecto y, si se violan, comprometen su integridad o propósito. El agente los tratará como restricciones absolutas al generar especificaciones, planes y código.

Úsalo cuando el principio:

- Es una restricción de seguridad, privacidad o cumplimiento normativo que no admite flexibilidad.
- Define una decisión técnica central que no puede revertirse sin reescribir el proyecto (por ejemplo, el mecanismo de persistencia o el lenguaje de programación).
- Establece una disciplina de calidad que, si se omite aunque sea una vez, invalida el proceso completo (por ejemplo, "tests primero").

Ejemplo de principio marcado:

```text
III. Test-First (NON-NEGOTIABLE)

Los tests deben escribirse antes de la implementación. El ciclo Red-Green-Refactor es obligatorio sin excepciones.
```

Reserva `(NON-NEGOTIABLE)` para lo que realmente no es negociable. Abusar del marcador reduce su efectividad y puede hacer que el agente genere artefactos demasiado rígidos para adaptarse a cambios legítimos.

#### Ejemplo de texto para crear la constitución

> *Quiero crear la constitución para TodoLite, una CLI mínima de gestión de tareas que soporta los comandos \`add\`, \`list\`, \`done\` y \`rm\`. Los principios del proyecto son: el código debe ser siempre ejecutable y estar cubierto por tests automatizados; la persistencia de datos debe realizarse en un archivo JSON (por defecto \`todolite.json\`, configurable mediante \`--file\`); el proyecto debe seguir las convenciones estándar del lenguaje* `<elige entre Python o C#>` *y las operaciones deben tener códigos de salida claros: 0 para éxito, 1 para recurso no encontrado y 2 para error de uso.*

**Artefacto esperado**: `constitution.md` en el directorio raíz del proyecto.

### 4. Crear la especificación

Ejecuta:

```text
/speckit.specify
```

Describe el escenario:

> "Construir una CLI mínima de tareas llamada TodoLite que soporte los comandos add, list (con filtro --open), done y rm. La persistencia debe ser en un archivo JSON (por defecto todolite.json, configurable con --file). Códigos de salida: 0 éxito, 1 no encontrado, 2 error de uso."

**Artefacto esperado**: `spec.md` con requisitos funcionales, escenarios de aceptación y criterios de éxito.

### 5. (Opcional) Clarificar la especificación

Ejecuta:

```text
/speckit.clarify
```

Este paso identifica áreas sub-especificadas en tu spec. El agente hará hasta 5 preguntas dirigidas. Responde y revisa cómo se actualizan las secciones de la especificación. Esta acción la puedes repetir tantas veces como quieras para iterar sobre la claridad y completitud de la especificación, sin embargo ten encuenta que cada iteración consume tiempo y tokens, con lo cual es importante que sepas discernir cuando parar.

La acción de `clarify` no requiere que suministres más información que la de hacer la llamada al agente correspondiente. Una técnica habitual es emplear modelos razonantes diferentes de los modelos empleados para crear las especificaciones, de esta forma se obtiene una revisión más crítica y profunda de las mismas. Por ejemplo, si empleaste GPT-5.4 al ejecutar `specify`, puedes emplear Claude Opus 4.6 para hacer la clarificación.

**Artefacto esperado**: `spec.md` actualizado con una sección de Clarificaciones.

### 6. Crear el plan de implementación

Ejecuta:

```text
/speckit.plan
```

**Artefacto esperado**: entre otros, el principal es el `plan.md` con stack técnico, decisiones de diseño y estrategia de testing. Sin embargo, dependiendo del proyecto otros archivos como `research.md` o `data-model.md` también pueden ser artefactos clave generados en esta fase.

### 7. (Opcional) Generar un checklist de verificación

Ejecuta:

```text
/speckit.checklist
```

Genera un checklist personalizado para verificar la completitud de tu especificación.

**Artefacto esperado**: Archivo en el directorio `checklists/` con ítems de verificación.

### 8. Generar las tareas

Ejecuta:

```text
/speckit.tasks
```

**Artefacto esperado**: `tasks.md` con una lista ordenada y accionable de tareas derivadas del plan.

### 9. (Opcional) Analizar consistencia cruzada

Ejecuta:

```text
/speckit.analyze
```

Verifica la consistencia entre spec, plan y tasks. Revisa los hallazgos y resuelve cualquier desalineación.

Responde y revisa cómo se actualizan las secciones de la especificación. Esta acción la puedes repetir tantas veces como quieras para iterar sobre la claridad y completitud de la especificación, sin embargo ten encuenta que cada iteración consume tiempo y tokens, con lo cual es importante que sepas discernir cuando parar. Una norma general es que se puede "sobrevivir" con algunas inconsistencias sobre todo de nivel "MEDIUM" o "LOW", pero las inconsistencias de nivel "HIGH" o "CRITICAL" deberían ser resueltas antes de avanzar a la implementación.

A veces, después de este paso, puede ser necesario pedirle al asistente de codificación (GitHub Copilot) que vuelva a revisar las listas de verificaciones creadas en el paso 7 de checklist para que las actualice con los nuevos hallazgos encontrados en el análisis. Esto se puede hacer fácilmente con un prompt directo al asistente de codificación, por ejemplo:

```text
Actualiza las listas de verificación en base a los hallazgos del análisis de consistencia.
```

**Artefacto esperado**: Reporte de análisis confirmando alineación o listando acciones de remediación.

### 10. Implementar

Ejecuta:

```text
/speckit.implement
```

O sigue las tareas manualmente usando el código de referencia como guía.

## Verificación

### Ruta C\#

```bash
cd exercises/greenfield/csharp
dotnet build
dotnet test
dotnet run --project src/TodoLite.Cli -- add "Comprar leche"
dotnet run --project src/TodoLite.Cli -- list
dotnet run --project src/TodoLite.Cli -- done 1
dotnet run --project src/TodoLite.Cli -- list
dotnet run --project src/TodoLite.Cli -- rm 1
```

**Posible salida después de `list` (antes de `done`)**:

```text
[ ]   1  Comprar leche
```

**Posible salida después de `list` (antes de `done`)**:

```text
[ ]   1  Comprar leche
```

**Posible salida después de `done 1` y `list`**:

```text
[x]   1  Comprar leche
```

### Ruta Python

```bash
cd exercises/greenfield/python
pip install -e ".[dev]"
pytest
python -m todolite add "Comprar leche"
python -m todolite list
python -m todolite done 1
python -m todolite list
python -m todolite rm 1
```

**Salida esperada**: Idéntica a la ruta C#.

## Código de referencia (respaldo)

Si tu código generado no funciona o el tiempo se agota, puedes copiar directamente el código de referencia:

### C\#

```bash
# Copiar el proyecto de referencia completo
cp -r exercises/greenfield/csharp/ todolite-csharp/
cd todolite-csharp
dotnet build
dotnet test
```

### Python

```bash
# Copiar el proyecto de referencia completo
cp -r exercises/greenfield/python/ todolite-python/
cd todolite-python
pip install -e ".[dev]"
pytest
```

## Resumen de artefactos SDD generados

| Paso | Comando | Artefacto |
| :--- | :--- | :--- |
| Constitución | `/speckit.constitution` | `constitution.md` |
| Especificación | `/speckit.specify` | `spec.md` |
| Clarificación | `/speckit.clarify` | `spec.md` actualizado |
| Plan | `/speckit.plan` | `plan.md` |
| Checklist | `/speckit.checklist` | `checklists/*.md` |
| Tareas | `/speckit.tasks` | `tasks.md` |
| Análisis | `/speckit.analyze` | Reporte de consistencia |
| Implementación | `/speckit.implement` | Código ejecutable + tests |

## Criterios de éxito

- [ ] Todos los artefactos SDD existen en tu directorio de feature
- [ ] La aplicación compila sin errores
- [ ] Todos los tests pasan
- [ ] Los 4 comandos CLI (`add`, `list`, `done`, `rm`) producen la salida esperada
- [ ] Al menos un paso opcional (clarify, checklist o analyze) fue ejecutado
