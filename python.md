Here are the most popular and reliable ways to package a Python script into a standalone executable (`.exe` on Windows, standalone app on macOS/Linux) as of late 2025.

### 1. PyInstaller (Most Popular & Recommended)
Works on Windows, macOS, and Linux. Creates a single folder or single file executable. Handles almost all libraries automatically.

#### Installation
```bash
pip install pyinstaller
```

#### Basic commands
```bash
# One-folder mode (easier to debug)
pyinstaller --onefile your_script.py

# OR one-file mode (single .exe / app)
pyinstaller --onefile your_script.py

# With a window (no console) - useful for GUI apps
pyinstaller --onefile --windowed your_script.py

# Add an icon
pyinstaller --onefile --windowed --icon=app.ico your_script.py
```

#### Common useful options
| Option                    | Purpose                                      |
|---------------------------|----------------------------------------------|
| `--onefile`               | Packs everything into a single executable   |
| `--windowed`              | No console window (for tkinter, PyQt, etc.) |
| `--add-data "file.txt;."` | Include extra files (Windows syntax)        |
| `--add-data "file.txt:."` | Include extra files (macOS/Linux syntax)    |
| `--hidden-import modulename` | For dynamically imported modules         |
| `--clean`                 | Clean PyInstaller cache before building     |

**Example with data files and hidden imports:**
```bash
pyinstaller --onefile --windowed \
    --add-data "assets/icon.png:assets" \
    --add-data "config.yaml:." \
    --hidden-import pandas \
    --hidden-import sqlalchemy \
    myscript.py
```

### 2. cx_Freeze (Good alternative)
```bash
pip install cx_Freeze
```

Create a `setup.py`:
```python
from cx_Freeze import setup, Executable

executables = [Executable("your_script.py", base="Win32GUI" if sys.platform == "win32" else None)]

setup(
    name="MyApp",
    version="1.0",
    description="My Python app",
    executables=executables
)
```

Then build:
```bash
python setup.py build
```

### 3. Nuitka (Fastest executables, compiles to C)
Produces the fastest and smallest executables because it compiles Python to C.

```bash
pip install nuitka
nuitka --onefile --windows-disable-console your_script.py    # Windows
nuitka --onefile your_script.py                              # macOS/Linux
```

Pros: Very fast startup and runtime  
Cons: Longer compile time, sometimes trickier with complex packages

### 4. Briefcase (by BeeWare) – Best for cross-platform apps with native look
Great if you want proper macOS .app, Windows .msi, Linux AppImage, etc.
https://briefcase.readthedocs.io/

### Quick Comparison (2025)

| Tool         | Single File | Speed     | Size      | Ease of Use | Best For                  |
|--------------|-------------|-----------|-----------|-------------|---------------------------|
| PyInstaller  | Yes         | Good      | Large     | ★★★★★      | Most projects             |
| Nuitka       | Yes         | Excellent | Small     | ★★★★       | Performance-critical      |
| cx_Freeze    | No (folder) | Good      | Medium    | ★★★★       | Simple scripts            |
| Briefcase    | Proper installers | Good  | Medium    | ★★★        | Professional apps         |

### Pro Tips
- Always test the executable on a clean machine (or VM) without Python installed.
- Use `--clean` with PyInstaller to avoid stale cache issues.
- For large apps (with pandas, numpy, etc.), expect 50–200+ MB executables.
- Use virtual environments to avoid pulling in unnecessary packages.
- Sign your executables on Windows/macOS for better security warnings.

**Recommendation for 2025**:  
→ Use **PyInstaller** for 90% of cases  
→ Switch to **Nuitka** if you need maximum speed or smaller size

Let me know your OS, whether it's a GUI app, and which libraries you use — I can give you the exact command!
