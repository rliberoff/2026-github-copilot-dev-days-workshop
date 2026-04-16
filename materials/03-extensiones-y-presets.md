# Extensiones y Presets

**Duración estimada**: 25 minutos

**Objetivo**: Aprender a personalizar Spec Kit mediante presets (plantillas) y extensiones (comandos), y comprender la pila de prioridad de resolución de plantillas.

## Requisitos previos

- Ejercicio greenfield completado o familiaridad con el flujo SDD
- Spec Kit inicializado en un proyecto (`specify init --here --integration copilot --force`)

## Parte 1: Presets — Personalizar plantillas

### ¿Qué es un preset?

Un preset es un paquete de personalización que modifica las plantillas de Spec Kit sin alterar la funcionalidad principal. Está definido por un archivo `preset.yml`.

### Estructura del preset

```text
dotnet-workshop-lite-preset/
├── preset.yml              # Manifiesto
├── README.md               # Documentación
├── LICENSE                 # Licencia
├── templates/
│   ├── spec-template.md    # Plantilla de spec personalizada
│   └── plan-template.md    # Plantilla de plan personalizada
└── commands/
    └── speckit.plan.md     # Override del comando plan
```

### Paso 1: Instalar el preset

```bash
specify preset add --dev exercises/presets/dotnet-workshop-lite-preset --priority 5
```

### Paso 2: Verificar la instalación

**`specify preset list`** muestra todos los presets registrados en el proyecto actual junto con su nombre, versión y prioridad numérica. Una prioridad más baja indica mayor precedencia: un preset con prioridad 1 gana a uno con prioridad 5 cuando ambos definen la misma plantilla. Úsalo para confirmar que `dotnet-workshop-lite-preset` aparece en la lista con la prioridad 5 asignada en el paso anterior.

```bash
specify preset list
```

**`specify preset resolve spec-template`** recorre la pila de prioridad de resolución de plantillas (overrides → presets → extensiones → core) y devuelve el contenido final de `spec-template` tal como lo usaría `/speckit.specify` en tiempo de ejecución. No genera ningún archivo: solo muestra en pantalla la plantilla resultante de la fusión. Con el preset activo, la salida debe incluir la sección **"Verifiable Acceptance Criteria"**, que el preset inyecta sobre la plantilla core de Spec Kit.

```bash
specify preset resolve spec-template
```

**`specify preset resolve plan-template`** aplica la misma lógica de resolución para la plantilla `plan-template` utilizada por `/speckit.plan`. Con el preset activo, la salida debe contener la tabla **"Key Decisions"**, que el preset añade para que el agente registre las decisiones de diseño y arquitectura más relevantes del ciclo de planificación.

```bash
specify preset resolve plan-template
```

### Paso 3: Ejercitar el preset con el flujo SDD

Ejecuta `/speckit.specify` describiendo: "Add a health-check endpoint that returns 200 OK with a JSON body containing service name and uptime."

Luego ejecuta `/speckit.plan`.

**Verificación**: El `spec.md` generado debe contener la sección **"Verifiable Acceptance Criteria"** y el `plan.md` debe contener la tabla **"Key Decisions"**.

## Parte 2: Extensiones — Agregar comandos

### ¿Qué es una extensión?

Una extensión agrega nuevos comandos slash, hooks o integraciones a Spec Kit. Está definida por un archivo `extension.yml`.

### Estructura de la extensión

```text
my-ext/
├── extension.yml                   # Manifiesto
├── README.md                       # Documentación
├── LICENSE                         # Licencia
├── templates/
│   └── spec-template.md            # Plantilla con sección Risk Assessment
├── commands/
│   ├── dotnet.quickcheck.md        # Comando de verificación .NET
│   └── python.quickcheck.md        # Comando de verificación Python
└── sample/
    ├── csharp/                     # Proyecto C# de prueba
    └── python/                     # Proyecto Python de prueba
```

### Paso 1: Instalar la extensión

```bash
specify extension add --dev exercises/extensions/my-ext
```

### Paso 2: Verificar la instalación

```bash
# Listar extensiones instaladas
specify extension list

# Ver metadatos
specify extension info my-ext
```

### Paso 3: Ejecutar el comando de verificación

#### Ruta C\#

Ejecuta en el chat del agente:

```bash
/speckit.my-ext.dotnet-quickcheck
```

Apunta al proyecto de ejemplo en `exercises/extensions/my-ext/sample/csharp/`.

**Salida esperada**:

```text
## .NET Quick Check Report

**Project**: Sample.sln
**Build**: PASS
**Tests**: 1 passed / 0 failed / 0 skipped
**Result**: PASS
```

#### Ruta Python

Ejecuta en el chat del agente:

```bash
/speckit.my-ext.python-quickcheck
```

Apunta al proyecto de ejemplo en `exercises/extensions/my-ext/sample/python/`.

**Salida esperada**:

```text
## Python Quick Check Report

**Project**: sample-greeter
**Install**: PASS
**Tests**: 1 passed / 0 failed / 0 skipped
**Result**: PASS
```

## Parte 3: Pila de prioridad de resolución de plantillas (FR-016)

Spec Kit resuelve plantillas siguiendo una pila de 4 capas de prioridad:

```text
1. Overrides (local)     → .specify/overrides/templates/
2. Presets                → Presets instalados (por prioridad)
3. Extensions             → Extensiones instaladas
4. Core                   → Plantillas base de Spec Kit
```

La capa con mayor prioridad gana. Vamos a demostrarlo paso a paso.

### Paso 1: Estado actual (preset + extensión instalados)

```bash
specify preset resolve spec-template
```

**Resultado**: La plantilla incluye la sección **"Verifiable Acceptance Criteria"** del preset (capa 2 gana sobre capa 3).

### Paso 2: Crear un override local (capa 1)

El archivo `.specify/overrides/templates/spec-template.md` ya existe en el repositorio. Incluye un **"Project Banner"** como encabezado.

```bash
specify preset resolve spec-template
```

**Resultado**: La plantilla ahora muestra el **"Project Banner"** del override local (capa 1 gana sobre todas).

### Paso 3: Eliminar el override local

Elimina o renombra el archivo de override:

```bash
mv .specify/overrides/templates/spec-template.md .specify/overrides/templates/spec-template.md.bak
```

```bash
specify preset resolve spec-template
```

**Resultado**: La plantilla vuelve a mostrar la sección **"Verifiable Acceptance Criteria"** del preset (capa 2).

### Paso 4: Eliminar el preset (rollback)

```bash
specify preset remove dotnet-workshop-lite
specify preset list
specify preset resolve spec-template
```

**Resultado**: `specify preset list` ya no muestra el preset. La plantilla ahora muestra la sección **"Risk Assessment"** de la extensión (capa 3).

### Paso 5: Eliminar la extensión (rollback)

```bash
specify extension remove my-ext
specify extension list
specify preset resolve spec-template
```

**Resultado**: `specify extension list` ya no muestra la extensión. Los comandos `/speckit.my-ext.dotnet-quickcheck` y `/speckit.my-ext.python-quickcheck` ya no están disponibles. La plantilla resuelve a la plantilla **core** de Spec Kit (capa 4).

## Resumen de la pila de prioridad

| Paso | Capa activa | Sección visible | Comando de verificación |
|------|-------------|----------------|------------------------|
| Override instalado | 1. Overrides | Project Banner | `specify preset resolve spec-template` |
| Override eliminado | 2. Presets | Verifiable Acceptance Criteria | `specify preset resolve spec-template` |
| Preset eliminado | 3. Extensions | Risk Assessment | `specify preset resolve spec-template` |
| Extensión eliminada | 4. Core | Plantilla base | `specify preset resolve spec-template` |

## Criterios de éxito

- [ ] El preset se instala y aparece en `specify preset list`
- [ ] `/speckit.specify` + `/speckit.plan` producen artefactos con las secciones personalizadas
- [ ] La extensión se instala y aparece en `specify extension list`
- [ ] `/speckit.my-ext.dotnet-quickcheck` produce un reporte PASS contra el proyecto C# de ejemplo
- [ ] `/speckit.my-ext.python-quickcheck` produce un reporte PASS contra el proyecto Python de ejemplo
- [ ] La pila de prioridad de 4 capas fue verificada paso a paso
- [ ] El rollback elimina preset y extensión sin residuos
