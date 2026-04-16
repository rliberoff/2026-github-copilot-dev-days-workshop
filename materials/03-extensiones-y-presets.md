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

Ejecuta `/speckit.specify` describiendo: "*Agrega un endpoint de health-check que devuelva 200 OK con un cuerpo JSON que contenga el nombre del servicio y el tiempo de actividad (uptime).*"

Luego ejecuta `/speckit.plan`.

**Verificación**: El `spec.md` generado debe contener la sección **"Verifiable Acceptance Criteria"**, y el `plan.md` debe contener la tabla **"Key Decisions"**.

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
│   └── spec-template.md            # Plantilla modificada
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

**Ejemplo de salida esperada**:

```text
## .NET Quick Check Report

**Project**: <nombre de la solución>.sln
**Build**: PASS
**Tests**: ... passed / 0 failed / 0 skipped
**Result**: PASS
```

#### Ruta Python

Ejecuta en el chat del agente:

```bash
/speckit.my-ext.python-quickcheck
```

Apunta al proyecto de ejemplo en `exercises/extensions/my-ext/sample/python/`.

**Ejemplo de salida esperada**:

```text
## Python Quick Check Report

**Project**: <nombre del proyecto>
**Install**: PASS
**Tests**: ... passed / 0 failed / 0 skipped
**Result**: PASS
```

## Criterios de éxito

- [ ] El preset se instala y aparece en `specify preset list`
- [ ] `/speckit.specify` + `/speckit.plan` producen artefactos con las secciones personalizadas
- [ ] La extensión se instala y aparece en `specify extension list`
- [ ] `/speckit.my-ext.dotnet-quickcheck` produce un reporte PASS contra el proyecto C# de ejemplo
- [ ] `/speckit.my-ext.python-quickcheck` produce un reporte PASS contra el proyecto Python de ejemplo
