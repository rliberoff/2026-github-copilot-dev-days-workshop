# Ejercicio Brownfield: Notes API

**Duración estimada**: 25 minutos

**Objetivo**: Aplicar SDD a un proyecto existente, agregando una nueva funcionalidad a una API que ya funciona.

## Premisa del escenario

> Tienes una API de notas (Notes API) funcional con *endpoints* de salud, creación, listado y obtención por ID. Tu tarea es agregar un *endpoint* de búsqueda `GET /notes/search` que filtre por texto (*query string* `q`) y por etiqueta (*query string* `tag`).

Los participantes generan los artefactos SDD en vivo durante la sesión. El código de referencia en el repositorio sirve como respaldo.

## Requisitos previos

- Entorno verificado (ver [00-preparacion.md](00-preparacion.md))
- Ejercicio greenfield completado (recomendado para contexto)
- VS Code con GitHub Copilot activo

## Diferencias con el ejercicio greenfield

Antes de comenzar, considera estas diferencias clave:

| Aspecto | Greenfield | Brownfield |
| :--- | :--- | :--- |
| Punto de partida | Directorio vacío | Proyecto existente con código funcional |
| Riesgo principal | Decisiones de diseño iniciales | No romper lo que ya funciona |
| Primer paso SDD | Definir todo desde cero | Entender el contexto existente primero |
| Verificación | Solo el nuevo código | Nuevo código + regresión del existente |

## Paso a paso

### 1. Verificar que el proyecto base funciona

#### Ruta C\#

```bash
cd exercises/brownfield/csharp
dotnet build
dotnet test
```

Los 9 tests deben pasar. Esto confirma que la API base (health, CRUD, search) funciona correctamente.

#### Ruta Python

```bash
cd exercises/brownfield/python
pip install -e ".[dev]"
pytest
```

Los 9 tests deben pasar.

### 2. Inicializar Spec Kit en el proyecto

```bash
specify init --here --integration copilot --force
```

Según estés en Windows o MacOS/Linux, el asistente de configuración de Spec Kit te preguntará si los *scripts* deben ser PowerShell (`ps`) o Bash (`sh`). Elige la opción que corresponda a tu entorno.

Nota que Spec Kit se inicializa sin alterar el código existente. Solo se crea el directorio `.specify/`.

### 3. Crear la constitución

Ejecuta:

```bash
/speckit.constitution
```

Proporciona principios relevantes para un proyecto brownfield:

- No romper endpoints existentes
- Mantener compatibilidad hacia atrás
- Agregar tests para la nueva funcionalidad

Para este ejercicio, podrías decir algo como:

> *Notes API es una API REST sin persistencia en disco que gestiona notas de texto en memoria, implementada con* <elige entre Python o C#> *. Actualmente expone cuatro endpoints: health check (\`GET /\`), creación de notas con validación de título obligatorio (\`POST /notes\`), listado completo ordenado por fecha descendente (\`GET /notes\`), y recuperación por identificador UUID (\`GET /notes/{id}\`). Las notas almacenan título, cuerpo, etiquetas normalizadas (minúsculas, sin duplicados, ordenadas) y marca de tiempo UTC. Como este es un proyecto existente es fundamental y no negociable que se preserve la funcionalidad actual (a menos que explícitamente se indique lo contrario) preservando la compatibilidad hacia atrás. Cualquier nueva funcionalidad debe incluir los correspondites test y pruebas.*

### 4. Especificar la nueva funcionalidad

Ejecuta:

```bash
/speckit.specify
```

Describe el escenario:

> *Agregar un nuevo endpoint de búsqueda \`GET /notes/search\` que acepte parámetros por query string \`q\` (texto) y \`tag\` (etiqueta). La búsqueda por texto debe ser case-insensitive en título y cuerpo. La búsqueda por \`tag\` debe usar etiquetas normalizadas. Cuando ambos parámetros están presentes, aplica lógica AND. Responde con un objeto \`SearchResult\` que incluya \`query\`, \`tag\`, \`count\` e \`items\`. El elemento \`items\` es una lista de notas que coinciden, ordenadas por fecha de creación descendente con sus correspondientes campos de \`id\`, \`title\`, \`body\`, \`tags\` y \`createdAtUtc\`. Si no se pasan parámetros, retorna todas las notas.*

### 5. (Opcional) Clarificar si hay ambigüedades

Si la especificación tiene áreas poco claras, ejecuta:

```bash
/speckit.clarify
```

Esto es especialmente útil en *brownfield* donde hay más contexto implícito que en greenfield.

Recuerda, este paso identifica áreas sub-especificadas en tu spec. El agente hará hasta 5 preguntas dirigidas. Responde y revisa cómo se actualizan las secciones de la especificación. Esta acción la puedes repetir tantas veces como quieras para iterar sobre la claridad y completitud de la especificación, sin embargo ten encuenta que cada iteración consume tiempo y tokens, con lo cual es importante que sepas discernir cuando parar.

La acción de `clarify` no requiere que suministres más información que la de hacer la llamada al agente correspondiente. Una técnica habitual es emplear modelos razonantes diferentes de los modelos empleados para crear las especificaciones, de esta forma se obtiene una revisión más crítica y profunda de las mismas. Por ejemplo, si empleaste GPT-5.4 al ejecutar `specify`, puedes emplear Claude Opus 4.6 para hacer la clarificación.

### 6. Crear el plan

Ejecuta:

```bash
/speckit.plan
```

### 7. (Opcional) Generar checklist

Ejecuta:

```bash
/speckit.checklist
```

Recuerda que este paso opcional genera un checklist personalizado (a veces varios archivos) para verificar la completitud de tu especificación.

### 8. Generar las tareas

Ejecuta:

```bash
/speckit.tasks
```

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

### 10. Implementar

Ejecuta:

```bash
/speckit.implement
```

## Verificación

### Ruta C\#

```bash
cd exercises/brownfield/csharp
dotnet build
dotnet test
```

Los tests deben pasar (incluyendo los tests de búsqueda).

Para probar manualmente, inicia el servidor:

```bash
dotnet run --project src/Notes.Api
```

En otra terminal:

```bash
# Crear una nota de prueba
curl -s -X POST "http://localhost:5000/notes" ^
  -H "Content-Type: application/json" ^
  -d "{\"title\":\"Comprar leche\",\"body\":\"Semi-descremada\",\"tags\":[\"compras\",\"hogar\"]}"

# Buscar por texto
curl -s "http://localhost:5000/notes/search?q=leche"

# Buscar por tag
curl -s "http://localhost:5000/notes/search?tag=compras"

# Búsqueda combinada
curl -s "http://localhost:5000/notes/search?q=leche&tag=compras"
```

### Ruta Python

```bash
cd exercises/brownfield/python
pip install -e ".[dev]"
pytest
```

Los tests deben pasar.

Para probar manualmente:

```bash
python -m notes_api
```

En otra terminal, usa los mismos comandos `curl` que en la ruta C#.

## Código de referencia (respaldo)

Si tu código generado no funciona o el tiempo se agota:

### C\#

El código de referencia completo está en `exercises/brownfield/csharp/`. Puedes copiarlo directamente:

```bash
# Copiar el proyecto de referencia
cp -r exercises/brownfield/csharp/ mi-notes-api-csharp/
cd mi-notes-api-csharp
dotnet build
dotnet test
```

### Python

El código de referencia completo está en `exercises/brownfield/python/`:

```bash
# Copiar el proyecto de referencia
cp -r exercises/brownfield/python/ mi-notes-api-python/
cd mi-notes-api-python
pip install -e ".[dev]"
pytest
```

## Contrato del endpoint de búsqueda

### `GET /notes/search`

**Parámetros de query**:

- `q` (string, opcional): Texto a buscar (case-insensitive en título y cuerpo)
- `tag` (string, opcional): Filtro por etiqueta (normalizada)

**Respuesta** (`200 OK`):

```json
{
  "query": "leche",
  "tag": "compras",
  "count": 1,
  "items": [
    {
      "id": "uuid-aqui",
      "title": "Comprar leche",
      "body": "Semi-descremada",
      "tags": ["compras", "hogar"],
      "createdAtUtc": "2026-04-13T10:00:00+00:00"
    }
  ]
}
```

**Comportamiento**:

- Sin parámetros → retorna todas las notas
- Solo `q` → filtra por texto en título/cuerpo
- Solo `tag` → filtra por etiqueta normalizada
- Ambos → lógica AND
- Resultados ordenados por `createdAtUtc` descendente

## Reflexión: Greenfield vs Brownfield

Al finalizar, reflexiona sobre estas preguntas:

1. ¿Qué fue más desafiante al especificar una nueva funcionalidad para código existente?
2. ¿Cómo cambió tu enfoque de testing sabiendo que ya había código en producción?
3. ¿Qué riesgos son exclusivos del escenario brownfield?
4. ¿En qué paso del flujo SDD notaste la mayor diferencia entre ambos enfoques?

## Criterios de éxito

- [ ] El proyecto base compila y sus tests pasan antes de cualquier cambio
- [ ] Spec Kit se inicializa sin romper el código existente
- [ ] Los artefactos SDD se generan correctamente para la nueva funcionalidad
- [ ] El endpoint de búsqueda funciona según el contrato
- [ ] Los tests existentes siguen pasando (sin regresión)
- [ ] Los nuevos tests de búsqueda también pasan
