# Preparación del Entorno

**Duración estimada**: 5 minutos

**Objetivo**: Verificar que todas las herramientas necesarias están instaladas y funcionando antes de comenzar los ejercicios.

## Requisitos compartidos (todos los participantes)

| Herramienta | Versión mínima | Comando de verificación | Instalación |
| :---: | :---: | :--- | :--- |
| Git | 2.x | `git --version` | [git-scm.com](https://git-scm.com/downloads) |
| PowerShell | 7.x | `pwsh --version` | Ver instrucciones abajo |
| Python | 3.11+ | `python --version` | [python.org](https://www.python.org/downloads/) |
| uv | última | `uv --version` | Ver instrucciones abajo |
| specify CLI | 0.6.x+ | `specify version` | `uv tool install specify-cli` |
| VS Code | última | Abre VS Code | [code.visualstudio.com](https://code.visualstudio.com/) |
| GitHub Copilot | última | Icono de Copilot en la barra lateral de VS Code | Extensión de VS Code Marketplace |

> **Importante para Windows**: Los comandos de este taller deben ejecutarse en **PowerShell 7 (`pwsh`)**. No uses **Windows PowerShell 5.1 (`powershell.exe`)**, que suele venir preinstalado por defecto en Windows.

## Requisitos por lenguaje

Cada participante debe verificar el runtime del lenguaje que haya elegido para los ejercicios.

| Ruta | Herramienta | Versión mínima | Comando de verificación | Instalación |
| :---: | :---: | :---: | :--- | :--- |
| C# | .NET SDK | 10.0+ | `dotnet --version` | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| Python | Python | 3.11+ | `python --version` | Ya cubierto en requisitos compartidos |

> **Nota**: No es necesario instalar ambos runtimes. Verifica solo el del lenguaje que vas a usar en este taller.

## Instrucciones de instalación

### PowerShell 7 (requerido en Windows)

Esta sección aplica a participantes de Windows. Usa uno de los siguientes métodos oficiales.

#### Método recomendado (winget)

1. Abre **Windows Terminal** (o `cmd`) como usuario normal.
2. Verifica que `winget` esté disponible:

```powershell
winget --version
```

1. Busca el paquete oficial:

```powershell
winget search --id Microsoft.PowerShell
```

1. Instala PowerShell 7 estable:

```powershell
winget install --id Microsoft.PowerShell --source winget
```

1. Cierra y vuelve a abrir la terminal.
2. Verifica la instalación:

```powershell
pwsh --version
```

Debes ver una versión `7.x`.

#### Método alternativo (instalador MSI oficial)

Si no tienes `winget`, usa el instalador MSI desde el release oficial de PowerShell:

1. Entra a [PowerShell Releases](https://github.com/PowerShell/PowerShell/releases/latest).
2. Descarga el MSI de tu arquitectura:
   - `PowerShell-*-win-x64.msi` (la más común)
   - `PowerShell-*-win-arm64.msi` (equipos ARM64)
3. Ejecuta el instalador y completa el asistente.
4. Cierra y vuelve a abrir la terminal.
5. Verifica con:

```powershell
pwsh --version
```

#### Confirmar que estás en PowerShell 7 y no en 5.1

Abre una sesión con `pwsh` y ejecuta:

```powershell
$PSVersionTable.PSVersion
```

El valor `Major` debe ser `7` o superior.

Si quieres comparar, en Windows PowerShell 5.1 suele aparecer `Major = 5`.

#### Referencia oficial

Consulta la documentación de Microsoft Learn: [Install PowerShell 7 on Windows](https://learn.microsoft.com/powershell/scripting/install/installing-powershell-on-windows).

### uv (gestor de paquetes Python)

#### Windows (PowerShell)

```powershell
irm https://astral.sh/uv/install.ps1 | iex
```

Ejecuta ese comando dentro de una sesión de **PowerShell 7 (`pwsh`)**.

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
pwsh --version
python --version
uv --version
specify version

# Solo si elegiste la ruta C#:
dotnet --version
```

**Resultado esperado**: Todos los comandos ejecutan sin error y muestran la versión instalada.

## Meta de tiempo

Este módulo debe completarse en **menos de 5 minutos**. Si algún prerequisito falla, consulta la guía de troubleshooting antes de continuar con los ejercicios.
