# Guía de Troubleshooting

**Propósito**: Referencia rápida para resolver problemas comunes durante el workshop. Cada resolución debe poder aplicarse en menos de 3 minutos.

## Tabla de diagnóstico rápido

| Síntoma | Causa probable | Resolución |
| :--- | :--- | :---: |
| `command not found: specify` | CLI no está en el PATH | Ver sección 1 |
| Comandos slash no aparecen en el agente | Agente no detectó los comandos | Ver sección 2 |
| `command not found: uv` o `python` no encontrado | uv o Python no instalado | Ver sección 3 |
| `dotnet: command not found` | .NET SDK no instalado | Ver sección 4 |
| Error al instalar preset o extensión | Incompatibilidad de versión | Ver sección 5 |
| Escaneo brownfield demasiado lento | Demasiados archivos en el proyecto | Ver sección 6 |
| Código del ejercicio diverge del esperado | Generación del agente difiere | Ver sección 7 |
| `detached HEAD` o rama incorrecta | Problema de Git | Ver sección 8 |
| Nombre de rama no sigue la convención | Convención de feature branch | Ver sección 9 |
| Cambios sin commit bloquean cambio de rama | Cambios pendientes en Git | Ver sección 10 |
| `UnicodeEncodeError` o `specify init` pide tipo de script | Consola Windows sin UTF-8 | Ver sección 11 |

---

## 1. `specify` no está en el PATH

### Síntoma

```
command not found: specify
'specify' is not recognized as an internal or external command
```

### Causa

El ejecutable de `specify` se instaló pero su directorio no está en la variable de entorno PATH.

### Resolución

#### Windows (PowerShell)

```powershell
# Verificar dónde se instaló
uv tool dir

# Agregar al PATH del usuario
$uvBin = (uv tool dir) -replace 'tools$','bin'
[Environment]::SetEnvironmentVariable("PATH", "$env:PATH;$uvBin", "User")

# Reiniciar PowerShell y verificar
specify version
```

#### macOS / Linux

```bash
# Agregar al PATH
echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.bashrc
source ~/.bashrc

# O para zsh:
echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc

# Verificar
specify version
```

Si sigue sin funcionar, reinstala:

```bash
uv tool install specify-cli --force
```

---

## 2. Comandos slash no aparecen en el agente

### Síntoma

Al escribir `/speckit.` en el chat del agente, no aparecen sugerencias de comandos.

### Causa

El agente no ha detectado los archivos de comando en `.specify/commands/` o `.specify/extensions/*/commands/`.

### Resolución

1. **Reiniciar el agente**: Cierra y vuelve a abrir la ventana de VS Code (`Ctrl+Shift+P` → "Reload Window").

2. **Verificar que los directorios existen**:

   ```bash
   ls .specify/commands/
   ls .specify/extensions/*/commands/
   ```

   Deben contener archivos `.md`.

3. **Re-ejecutar init con --force**:

   ```bash
   specify init --here --integration copilot --force
   ```

4. **Verificar que el agente correcto está activo**: Asegúrate de que GitHub Copilot esté activo en la barra lateral de VS Code (icono de Copilot visible).

---

## 3. uv o Python no instalado

### Síntoma

```
command not found: uv
Python was not found
python: command not found
```

### Causa

uv o Python 3.11+ no están instalados en el sistema.

### Resolución para uv

#### Windows

```powershell
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

#### macOS / Linux

```bash
curl -LsSf https://astral.sh/uv/install.sh | sh
```

Cierra y abre una nueva terminal después de instalar.

### Resolución para Python

```bash
# Instalar Python 3.11 con uv (si uv ya está disponible)
uv python install 3.11
```

O descarga desde [python.org/downloads](https://www.python.org/downloads/).

---

## 4. .NET SDK no encontrado

### Síntoma

```
dotnet: command not found
'dotnet' is not recognized as an internal or external command
```

### Causa

El .NET SDK 10.0 no está instalado o no está en el PATH.

### Resolución

#### Windows

```powershell
winget install Microsoft.DotNet.SDK.10
```

#### macOS

```bash
brew install dotnet-sdk
```

#### Linux (Ubuntu/Debian)

```bash
sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0
```

Después de instalar, cierra y abre una nueva terminal:

```bash
dotnet --version
```

---

## 5. Error al instalar preset o extensión

### Síntoma

```
Error: Incompatible version
Error: Failed to install preset/extension
```

### Causa

La versión de `specify` CLI es anterior a la requerida por el preset o extensión (`specifyMinVersion` en el manifiesto).

### Resolución

1. **Verificar la versión actual**:

   ```bash
   specify version
   ```

2. **Verificar la versión requerida**: Abre el archivo `preset.yml` o `extension.yml` y busca el campo `specifyMinVersion`.

3. **Actualizar el CLI**:

   ```bash
   uv tool install specify-cli --force
   ```

4. **Si la versión es correcta**, verifica que la ruta al preset/extensión es correcta:

   ```bash
   # Para preset
   specify preset add --dev exercises/presets/dotnet-workshop-lite-preset --priority 5

   # Para extensión
   specify extension add --dev exercises/extensions/my-ext
   ```

---

## 6. Escaneo brownfield demasiado lento

### Síntoma

El agente tarda demasiado al analizar el proyecto brownfield o se queda procesando.

### Causa

El proyecto contiene demasiados archivos (node_modules, bin, obj, etc.) que el agente intenta analizar.

### Resolución

1. **Verificar .gitignore**: Asegúrate de que los directorios pesados están excluidos:

   ```bash
   cat .gitignore
   ```

2. **Agregar exclusiones si faltan**:

   ```text
   node_modules/
   bin/
   obj/
   .venv/
   __pycache__/
   *.egg-info/
   *.lscache
   .pytest_cache/
   ```

3. **Reducir el contexto**: Si el agente sigue lento, enfoca el análisis en un subdirectorio específico:

   ```text
   /speckit.specify
   ```

   Y menciona explícitamente: "Analiza solo el directorio src/ del proyecto."

---

## 7. Código del ejercicio diverge del esperado

### Síntoma

El código generado por el agente no produce la salida esperada o no compila.

### Causa

La generación del agente puede variar entre sesiones. Esto es normal y esperado.

### Resolución

1. **Intentar regenerar**: Ejecuta `/speckit.implement` nuevamente con más contexto.

2. **Usar el código de referencia como respaldo**:

   ```bash
   # Greenfield C#
   cp -r exercises/greenfield/csharp/ mi-proyecto/

   # Greenfield Python
   cp -r exercises/greenfield/python/ mi-proyecto/

   # Brownfield C#
   cp -r exercises/brownfield/csharp/ mi-proyecto/

   # Brownfield Python
   cp -r exercises/brownfield/python/ mi-proyecto/
   ```

3. **Verificar que el código de referencia funciona**:

   ```bash
   # C#
   dotnet build && dotnet test

   # Python
   pip install -e ".[dev]" && pytest
   ```

---

## 8. `detached HEAD` o rama incorrecta

### Síntoma

```
You are in 'detached HEAD' state
HEAD is now at abc1234...
```

O estás en una rama diferente a la esperada.

### Causa

Se hizo `git checkout` a un commit específico en lugar de a una rama, o se navegó a la rama equivocada.

### Resolución

1. **Volver a la rama correcta**:

   ```bash
   # Ver en qué estado estás
   git status
   git branch

   # Cambiar a la rama del workshop
   git switch main
   # O si la rama tiene otro nombre:
   git switch 001-speckit-workshop
   ```

2. **Si tienes cambios que quieres conservar en detached HEAD**:

   ```bash
   # Crear una rama desde el estado actual
   git switch -c mi-rama-recuperada
   ```

---

## 9. Nombre de rama no sigue la convención

### Síntoma

La rama no sigue el patrón esperado (por ejemplo, `feature/001-speckit-workshop` vs `001-speckit-workshop`).

### Causa

La rama fue creada con un nombre diferente al esperado por la convención del proyecto.

### Resolución

```bash
# Ver el nombre actual
git branch --show-current

# Renombrar la rama actual
git branch -m nombre-actual nombre-correcto

# Ejemplo:
git branch -m mi-rama 001-speckit-workshop
```

---

## 10. Cambios sin commit bloquean cambio de rama

### Síntoma

```
error: Your local changes to the following files would be overwritten by checkout
Please commit your changes or stash them before you switch branches
```

### Causa

Hay archivos modificados sin commit que entran en conflicto con la rama destino.

### Resolución

**Opción A**: Guardar cambios temporalmente (stash):

```bash
git stash
git switch rama-destino

# Cuando quieras recuperar los cambios:
git stash pop
```

**Opción B**: Hacer commit de los cambios:

```bash
git add .
git commit -m "chore: save work in progress"
git switch rama-destino
```

---

## 11. `specify init` falla en Windows con error de codificación

### Síntoma

```
UnicodeEncodeError: 'charmap' codec can't encode characters in position 0-2
```

O el comando `specify init --here --integration copilot --force` entra en modo interactivo preguntando por el tipo de script.

### Causa

La consola de Windows usa la codificación `cp1252` por defecto, que no soporta los caracteres Unicode que emite Spec Kit. Además, en Windows es necesario indicar explícitamente el tipo de script (`ps` para PowerShell).

### Resolución

Ejecutar los siguientes comandos antes de `specify init`:

```powershell
chcp 65001
$env:PYTHONIOENCODING = 'utf-8'
$env:PYTHONUTF8 = '1'
```

Luego ejecutar init con el flag `--script ps`:

```powershell
specify init --here --integration copilot --force --script ps
```

Si el problema persiste, verificar que la terminal de VS Code está configurada como PowerShell (no CMD).

**Opción C**: Descartar los cambios (solo si no los necesitas):

```bash
git checkout -- .
git switch rama-destino
```
