# Ejercicio Brownfield: Notes API

**Duración estimada**: 25 minutos
**Objetivo**: Aplicar SDD a un proyecto existente, agregando una nueva funcionalidad a una API que ya funciona.

## Premisa del escenario

> Tienes una API de notas (Notes API) funcional con endpoints de salud, creación, listado y obtención por ID. Tu tarea es agregar un endpoint de búsqueda `GET /notes/search` que filtre por texto (query string `q`) y por etiqueta (`tag`).

Los participantes generan los artefactos SDD en vivo durante la sesión. El código de referencia en el repositorio sirve como respaldo.

## Requisitos previos

- Entorno verificado (ver [00-preparacion.md](00-preparacion.md))
- Ejercicio greenfield completado (recomendado para contexto)
- VS Code con GitHub Copilot activo

## Diferencias con el ejercicio greenfield

Antes de comenzar, considera estas diferencias clave:

| Aspecto | Greenfield | Brownfield |
|---------|------------|------------|
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

Nota que Spec Kit se inicializa sin alterar el código existente. Solo se crea el directorio `.specify/`.

### 3. Crear la constitución

Ejecuta:

```
/speckit.constitution
```

Proporciona principios relevantes para un proyecto brownfield:

- No romper endpoints existentes
- Mantener compatibilidad hacia atrás
- Agregar tests para la nueva funcionalidad

### 4. Especificar la nueva funcionalidad

Ejecuta:

```
/speckit.specify
```

Describe el escenario:

> "Agregar un endpoint de búsqueda GET /notes/search que acepte parámetros query `q` (texto) y `tag` (etiqueta). La búsqueda por texto debe ser case-insensitive en título y cuerpo. La búsqueda por tag debe usar tags normalizados. Cuando ambos parámetros están presentes, aplica lógica AND. Responde con un objeto SearchResult que incluya query, tag, count e items. Si no se pasan parámetros, retorna todas las notas."

### 5. (Opcional) Clarificar si hay ambigüedades

Si la especificación tiene áreas poco claras, ejecuta:

```
/speckit.clarify
```

Esto es especialmente útil en brownfield donde hay más contexto implícito que en greenfield.

### 6. Crear el plan

Ejecuta:

```
/speckit.plan
```

### 7. (Opcional) Generar checklist o analizar consistencia

Si quieres verificar la completitud de tu spec o la consistencia entre artefactos:

```
/speckit.checklist
/speckit.analyze
```

Estos pasos opcionales son particularmente valiosos en brownfield donde hay más riesgo de inconsistencias entre el código existente y la nueva especificación.

### 8. Generar las tareas

Ejecuta:

```
/speckit.tasks
```

### 9. Implementar

Ejecuta:

```
/speckit.implement
```

O implementa manualmente siguiendo las tareas generadas.

## Verificación

### Ruta C\#

```bash
cd exercises/brownfield/csharp
dotnet build
dotnet test
```

Los 9 tests deben pasar (incluyendo los tests de búsqueda).

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

Los 9 tests deben pasar.

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
