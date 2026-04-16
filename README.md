# GitHub Copilot Dev Days Workshop 2026

Taller (*workshop*) de **Desarrollo Guiado por Especificaciones (SDD)** con [GitHub Spec Kit](https://github.com/github/spec-kit). Los participantes aprenden a aplicar el flujo completo de SDD, desde la especificación hasta la implementación, usando la CLI `specify` y GitHub Copilot (de preferencia en VS Code).

## Objetivos

1. Aplicar el flujo completo de SDD (constitution → specify → plan → tasks → implement)
2. Distinguir entre especificación (qué / por qué) y plan (cómo)
3. Construir una aplicación desde cero (*greenfield*) usando SDD
4. Personalizar Spec Kit con presets y extensiones
5. Aplicar SDD a un proyecto existente (*brownfield*)
6. Identificar diferencias concretas entre adopción *greenfield* y *brownfield*

## Estructura del workshop

| Módulo | Duración | Material | Descripción |
| :--- | :--- | :--- | :--- |
| 0. Preparación | 5 min | [00-preparacion.md](materials/00-preparacion.md) | Verificación del entorno |
| 1. Introducción | 15 min | [01-introduccion.md](materials/01-introduccion.md) | Conceptos SDD, vibe coding vs context engineering |
| 2. Ejercicio Greenfield | 45 min | [02-ejercicio-greenfield.md](materials/02-ejercicio-greenfield.md) | Construir TodoLite CLI desde cero |
| 3. Extensiones y Presets | 25 min | [03-extensiones-y-presets.md](materials/03-extensiones-y-presets.md) | Personalizar plantillas y comandos |
| 4. Ejercicio Brownfield | 25 min | [04-ejercicio-brownfield.md](materials/04-ejercicio-brownfield.md) | Agregar búsqueda a Notes API |
| 5. Cierre | 5 min | [05-cierre.md](materials/05-cierre.md) | Retrospectiva y próximos pasos |
| **Total** | **~120 min** | | |

## Requisitos previos

### Compartidos (todos los participantes)

| Herramienta | Versión mínima | Verificación | Instalación |
| :--- | :--- | :--- | :--- |
| Git | 2.x | `git --version` | [git-scm.com](https://git-scm.com/downloads) |
| Python | 3.11+ | `python --version` | [python.org](https://www.python.org/downloads/) |
| uv | última | `uv --version` | `irm https://astral.sh/uv/install.ps1 \| iex` (Win) / `curl -LsSf https://astral.sh/uv/install.sh \| sh` (Mac/Linux) |
| specify CLI | 0.6.x+ | `specify version` | `uv tool install specify-cli` |
| VS Code | última | — | [code.visualstudio.com](https://code.visualstudio.com/) |
| GitHub Copilot | última | Icono en la barra lateral de VS Code | Extensión de VS Code Marketplace |

### Por lenguaje (elige uno)

| Ruta | Herramienta | Versión mínima | Verificación |
| :--- | :--- | :--- | :--- |
| C# | .NET SDK | 10.0+ | `dotnet --version` |
| Python | Python | 3.11+ | `python --version` |

### Verificación rápida

```bash
specify check
```

## Estructura del repositorio

```text
materials/                                  # Guías del workshop (en español)
├── 00-preparacion.md                       # Verificación del entorno
├── 01-introduccion.md                      # Conceptos SDD y Spec Kit
├── 02-ejercicio-greenfield.md              # Ejercicio: TodoLite CLI desde cero
├── 03-extensiones-y-presets.md             # Ejercicio: personalización de Spec Kit
├── 04-ejercicio-brownfield.md              # Ejercicio: agregar búsqueda a Notes API
├── 05-cierre.md                            # Retrospectiva y próximos pasos
├── troubleshooting.md                      # Resolución de problemas comunes
└── retrospectiva.md                        # Plantilla de retrospectiva

exercises/
├── greenfield/                             # Código de referencia: TodoLite CLI
│   ├── csharp/                     
│   │   ├── TodoLite.sln
│   │   ├── src/                    
    │   │   └── test/                   
│   └── python/                             
│       ├── pyproject.toml
│       ├── todolite/                       
│       └── tests/                          
├── brownfield/                             # Código de referencia: Notes API
│   ├── csharp/                             
│   │   ├── NotesApi.sln
│   │   ├── src/Notes.Api/
│   │   └── test/Notes.Api.Tests/
│   └── python/                             
│       ├── pyproject.toml
│       ├── notes_api/
│       └── tests/
├── extensions/my-ext/                      # Extensión de ejemplo con quickcheck commands
│   ├── extension.yml
│   ├── commands/                           
│   ├── templates/                          
│   └── sample/                             
└── presets/dotnet-workshop-lite-preset/    # Preset de ejemplo
    ├── preset.yml
    ├── templates/                          # Spec y plan templates personalizados
    └── commands/                           # Override del comando plan

specs/001-speckit-workshop/                 # Especificaciones SDD del workshop
├── spec.md                                 # Especificación funcional
├── plan.md                                 # Plan de implementación
├── tasks.md                                # Tareas derivadas del plan
├── data-model.md                           # Modelo de datos
├── research.md                             # Investigación técnica
├── quickstart.md                           # Guía rápida de verificación
├── checklists/                             # Listas de verificación
└── contracts/                              
```

## Ejercicios

### Greenfield: TodoLite CLI

Construye una CLI mínima de tareas desde cero con los comandos `add`, `list` (con filtro `--open`), `done` y `rm`. Persistencia en archivo JSON.

```bash
# C#
cd exercises/greenfield/csharp
dotnet build && dotnet test
dotnet run --project src/TodoLite.Cli -- add "Comprar leche"
dotnet run --project src/TodoLite.Cli -- list

# Python
cd exercises/greenfield/python
pip install -e ".[dev]"
pytest
python -m todolite add "Comprar leche"
python -m todolite list
```

### Brownfield: Notes API search endpoint

Agrega un endpoint `GET /notes/search` a una API de notas existente. Soporta búsqueda por texto (`q`) y etiqueta (`tag`) con lógica AND.

```bash
# C#
cd exercises/brownfield/csharp
dotnet build && dotnet test

# Python
cd exercises/brownfield/python
pip install -e ".[dev]"
pytest
```

### Extensiones y presets

Instala y verifica personalizaciones de Spec Kit:

```bash
# Instalar preset
specify preset add --dev exercises/presets/dotnet-workshop-lite-preset --priority 5
specify preset list

# Instalar extensión
specify extension add --dev exercises/extensions/my-ext
specify extension list
```

## Flujo SDD de Spec Kit

```text
 ┌──────────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌────────────┐
 │ constitution │───▶│ specify  │───▶│   plan   │───▶│  tasks   │───▶│ implement  │
 │              │    │          │    │          │    │          │    │            │
 │ Principios   │    │ spec.md  │    │ plan.md  │    │ tasks.md │    │ Código +   │
 │ del proyecto │    │ Qué/     │    │ Cómo     │    │ Lista    │    │ Tests      │
 │              │    │ Por qué  │    │          │    │          │    │            │
 └──────────────┘    └──────────┘    └──────────┘    └──────────┘    └────────────┘
```

| Comando | Artefacto | Propósito |
| :--- | :--- | :--- |
| `/speckit.constitution` | `constitution.md` | Define principios y restricciones del proyecto |
| `/speckit.specify` | `spec.md` | Requisitos, escenarios y criterios de aceptación |
| `/speckit.clarify` | `spec.md` (actualizado) | Identifica áreas sub-especificadas *(opcional)* |
| `/speckit.plan` | `plan.md` | Stack técnico, arquitectura y estrategia de testing |
| `/speckit.checklist` | `checklists/*.md` | Checklist de verificación *(opcional)* |
| `/speckit.tasks` | `tasks.md` | Lista ordenada de tareas derivadas del plan |
| `/speckit.analyze` | Reporte | Verifica consistencia entre artefactos *(opcional)* |
| `/speckit.implement` | Código + tests | Ejecuta las tareas y produce código funcional |

## Tecnologías

| Componente | C# | Python |
| :--- | :--- | :--- |
| Lenguaje | C# 14 / .NET 10 | Python 3.11+ |
| Greenfield | Console app, `System.Text.Json` | `argparse`, `json`, `dataclasses` |
| Brownfield | ASP.NET Minimal API | Flask 3.x |
| Testing | xUnit | pytest |
| Spec Kit | `specify` CLI 0.6.x+ | `specify` CLI 0.6.x+ |

## Troubleshooting

Consulta la [guía de troubleshooting](materials/troubleshooting.md) para resolución de problemas comunes, incluyendo:

- `specify` no está en el PATH
- Comandos slash no aparecen en el agente
- `uv` o Python no encontrado
- .NET SDK no instalado
- Errores al instalar presets o extensiones

## Licencia

Este proyecto está bajo la [licencia MIT](LICENSE).
