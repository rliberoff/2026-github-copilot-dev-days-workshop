# Workshop: Desarrollo Guiado por Especificaciones con GitHub Spec Kit

## Objetivos

Al finalizar este workshop, los participantes podrán:

1. Aplicar el flujo completo de Desarrollo Guiado por Especificaciones (SDD) usando Spec Kit
2. Distinguir entre especificación (qué/por qué) y plan (cómo)
3. Construir una aplicación desde cero (greenfield) usando SDD
4. Personalizar Spec Kit con presets y extensiones
5. Aplicar SDD a un proyecto existente (brownfield)
6. Identificar diferencias concretas entre adopción greenfield y brownfield

## Estructura y tiempos

| Módulo | Duración | Archivo | Descripción |
|--------|----------|---------|-------------|
| 0. Preparación | 5 min | [00-preparacion.md](00-preparacion.md) | Verificación del entorno |
| 1. Introducción | 15 min | [01-introduccion.md](01-introduccion.md) | Conceptos SDD y Spec Kit |
| 2. Ejercicio Greenfield | 45 min | [02-ejercicio-greenfield.md](02-ejercicio-greenfield.md) | Construir TodoLite CLI desde cero |
| 3. Extensiones y Presets | 25 min | [03-extensiones-y-presets.md](03-extensiones-y-presets.md) | Personalizar plantillas y comandos |
| 4. Ejercicio Brownfield | 25 min | [04-ejercicio-brownfield.md](04-ejercicio-brownfield.md) | Agregar búsqueda a Notes API |
| 5. Cierre | 5 min | [05-cierre.md](05-cierre.md) | Retrospectiva y próximos pasos |
| **Total** | **~120 min** | | |

## Requisitos previos (resumen)

- Git 2.x, Python 3.11+, uv, specify CLI 0.6.x+
- VS Code con GitHub Copilot
- .NET SDK 10.0+ (ruta C#) o Python 3.11+ (ruta Python)
- Detalle completo en [00-preparacion.md](00-preparacion.md)

## Materiales de soporte

- [Guía de troubleshooting](troubleshooting.md) — Resolución de problemas comunes
- [Plantilla de retrospectiva](retrospectiva.md) — Para completar durante el cierre
- [Código de referencia greenfield C#](../exercises/greenfield/csharp/)
- [Código de referencia greenfield Python](../exercises/greenfield/python/)
- [Código de referencia brownfield C#](../exercises/brownfield/csharp/)
- [Código de referencia brownfield Python](../exercises/brownfield/python/)
- [Preset de ejemplo](../exercises/presets/dotnet-workshop-lite-preset/)
- [Extensión de ejemplo](../exercises/extensions/my-ext/)

## Notas para el instructor

- Cada participante elige **una ruta de lenguaje** (C# o Python) para los ejercicios
- El repositorio y los materiales cubren ambas rutas
- Los participantes que terminen antes pueden explorar la segunda ruta
- El código de referencia está disponible como respaldo en cada ejercicio
- Consulta la [guía de troubleshooting](troubleshooting.md) para resolución rápida de problemas
