# Preparación del Entorno

**Duración estimada**: 5 minutos
**Objetivo**: Verificar que todas las herramientas necesarias están instaladas y funcionando antes de comenzar los ejercicios.

## Requisitos compartidos (todos los participantes)

| Herramienta | Versión mínima | Comando de verificación | Instalación |
| :---: | :---: | :--- | :--- |
| Git | 2.x | `git --version` | [git-scm.com](https://git-scm.com/downloads) |
| Python | 3.11+ | `python --version` | [python.org](https://www.python.org/downloads/) |
| uv | última | `uv --version` | Ver instrucciones abajo |
| specify CLI | 0.6.x+ | `specify version` | `uv tool install specify-cli` |
| VS Code | última | Abre VS Code | [code.visualstudio.com](https://code.visualstudio.com/) |
| GitHub Copilot | última | Icono de Copilot en la barra lateral de VS Code | Extensión de VS Code Marketplace |

## Requisitos por lenguaje

Cada participante debe verificar el runtime del lenguaje que haya elegido para los ejercicios.

| Ruta | Herramienta | Versión mínima | Comando de verificación | Instalación |
| :---: | :---: | :---: | :--- | :--- |
| C# | .NET SDK | 10.0+ | `dotnet --version` | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| Python | Python | 3.11+ | `python --version` | Ya cubierto en requisitos compartidos |

> **Nota**: No es necesario instalar ambos runtimes. Verifica solo el del lenguaje que vas a usar en este taller.

## Instrucciones de instalación

### uv (gestor de paquetes Python)

#### Windows (PowerShell)

```powershell
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

#### macOS / Linux

```bash
curl -LsSf https://astral.sh/uv/install.sh | sh
```

Después de instalar, cierra y vuelve a abrir la terminal para que el PATH se actualice.

### specify CLI

```bash
uv tool install specify-cli
```

Si ya está instalado y necesitas actualizar:

```bash
uv tool install specify-cli --force
```

### .NET SDK 10 (solo ruta C#)

Descarga e instala desde [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).

#### Windows

Usa el instalador `.exe` o con winget:

```powershell
winget install Microsoft.DotNet.SDK.10
```

#### macOS

Usa el instalador `.pkg` o con Homebrew:

```bash
brew install dotnet-sdk
```

#### Linux (Ubuntu/Debian)

```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
```

Para otras distribuciones, consulta la [documentación oficial](https://learn.microsoft.com/dotnet/core/install/linux).

### Python 3.11+ (si no está instalado)

#### Windows

Descarga desde [python.org](https://www.python.org/downloads/) o con winget:

```powershell
winget install Python.Python.3.11
```

#### macOS

```bash
brew install python@3.11
```

#### Linux (Ubuntu/Debian)

```bash
sudo apt-get update
sudo apt-get install -y python3.11 python3.11-venv
```

O usa uv para instalar una versión específica:

```bash
uv python install 3.11
```

## Verificación rápida

Ejecuta todos los comandos de verificación de una vez:

```bash
specify check
```

Este comando valida todas las herramientas requeridas y reporta su estado.

### Verificación manual completa

Si `specify check` no está disponible o quieres verificar individualmente:

```bash
git --version
python --version
uv --version
specify version

# Solo si elegiste la ruta C#:
dotnet --version
```

**Resultado esperado**: Todos los comandos ejecutan sin error y muestran la versión instalada.

## Meta de tiempo

Este módulo debe completarse en **menos de 5 minutos**. Si algún prerequisito falla, consulta la guía de troubleshooting antes de continuar con los ejercicios.
