# 🎨 Pixel Wall‑E Art

![Pixel Wall‑E Banner](https://i.imgur.com/3hX5QvT.png)

Un lenguaje de scripting ligero para crear **pixel‑art** sobre un canvas cuadriculado, junto con un intérprete y dos frontends (consola y GUI).

---

## 🚀 Características Destacadas

| 📦 Componente        | ✔️ Funcionalidad Principal                                      | 🎯 Objetivo                                                            |
|----------------------|-----------------------------------------------------------------|------------------------------------------------------------------------|
| 🖋️ **Lenguaje**      | Sintaxis: `Spawn`, `Color`, `Size`, `DrawLine`, `DrawCircle`, `DrawRectangle`, `Fill`, asignaciones, etiquetas y saltos (`GoTo[...]`) | Permitir dibujar figuras y controlar flujo con variables y bucles      |
| 🔍 **Análisis**      | Lexer → Parser → AST → Semántico → Intérprete                   | Separar fases de compilación e interpretación para robustez y extensibilidad |
| 🖥️ **Consola**       | `PixelWallE.Console` con `CompilerRunner` → lectura de `.pw`, generación de `output.png` | Ejecutar scripts desde terminal y obtener PNG final                    |
| 🖼️ **GUI WinForms**  | Editor de texto, botones Cargar/Guardar/Ejecutar, cuadricula, escalado nearest‑neighbor | Experiencia interactiva: ver tu código y resultado sin salir de la app  |
| 🛠️ **Extensible**    | Añade nuevos comandos, funciones y expresiones sin tocar lo existente | Facilitar nuevas instrucciones, operaciones o frontends               |

---

## 📜 Comandos y Sintaxis

| **Instrucción**                    | **Parámetros**                                    | **Efecto**                                                   |
|------------------------------------|---------------------------------------------------|--------------------------------------------------------------|
| `Spawn(x, y)`                      | `int x, int y`                                    | Inicializa a Wall‑E en `(x,y)`                                |
| `Color("name")`                    | `"Red"`, `"Blue"`, `"Green"`, `"Yellow"`, etc.    | Cambia color de brocha                                        |
| `Size(n)`                          | `n > 0` (impar)                                   | Ajusta grosor de brocha (si es par lo redondea al inferior)   |
| `DrawLine(dx, dy, dist)`           | `dx, dy ∈ {-1,0,1}`, `dist ≥ 0`                   | Dibuja línea en dirección `(dx, dy)`                          |
| `DrawCircle(dx, dy, r)`            | `dx, dy ∈ {-1,0,1}`, `r ≥ 0`                      | Dibuja circunferencia de radio `r`                            |
| `DrawRectangle(dx, dy, d, w, h)`   | `dx, dy ∈ {-1,0,1}`, `d ≥ 0`, `w,h ≥ 1`            | Dibuja y rellena rectángulo centrado a distancia `d`         |
| `Fill()`                           | —                                                 | Flood‑fill de región contigua                                 |
| `var <- Expression`                | literales, variables y operadores (`+ - * / % **`) | Asigna resultado de expresión                                 |
| `label` (línea sola)               | cadena alfanumérica                                | Define etiqueta para saltos                                   |
| `GoTo[label](condición)`           | `label`, `condición ∈ {bool}`                     | Salto condicional a etiqueta                                  |

---

## 🔧 Funciones Integradas

| Función                      | Parámetros & Retorno           | Descripción                                   |
|------------------------------|--------------------------------|-----------------------------------------------|
| `GetActualX()`               | — → `int`                      | Retorna `X` actual de Wall‑E                  |
| `GetActualY()`               | — → `int`                      | Retorna `Y` actual de Wall‑E                  |
| `GetCanvasSize()`            | — → `int`                      | Retorna tamaño `n` (canvas `n×n`)             |
| `GetColorCount(color,x1,y1,x2,y2)` | `string,int,int,int,int` → `int` | Cuenta píxeles de `color` en sub‑región      |
| `IsBrushColor(color)`        | `string` → `0/1`               | Comprueba color de brocha                     |
| `IsBrushSize(size)`          | `int` → `0/1`                  | Comprueba grosor de brocha                    |
| `IsCanvasColor(color,v,h)`   | `string,int,int` → `0/1`       | Comprueba color de píxel relativo             |

---

## 📂 Estructura de Carpeta

```
Wall‑E-Art-Project/
├─ Proyecto_Wall_E_Art/       # Lógica (lexer, parser, AST, semántico, intérprete)
├─ PixelWallE.Console/        # App consola (CompilerRunner, input.pw → output.png)
├─ PixelWallE.GUI/            # WinForms GUI (Form1, Program.cs, PixelPictureBox)
└─ README.md                  # Documentación (este fichero)
```

---

## 🛠 Instalación y Uso

### Prerrequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

### 1. Ejecución en consola

```bash
cd PixelWallE.Console
dotnet run -- input.pw
# Genera output.png con el dibujo resultante
```

### 2. Aplicación Windows Forms

```bash
cd PixelWallE.GUI
dotnet run
```

1. Escribe o carga (`.pw`) tu script.
2. Ajusta tamaño del canvas.
3. Pulsa **Ejecutar** para ver el resultado con cuadricula.

---

## 🧪 Casos de Prueba

En `PixelWallE.Console/input.pw` (y carpeta `tests/`) encontrarás scripts de ejemplo:

1. **LineaSimple.pw**  
   ```pw
   Spawn(0,0)
   DrawLine(1,0,10)
   ```
2. **VariablesLoop.pw**  
   ```pw
   Spawn(5,5)
   n <- 3
   loop
   DrawCircle(0,1,n)
   n <- n - 1
   GoTo[loop](n > 0)
   ```
3. **FloodFill.pw**  
   Dibuja un contorno y luego lo rellena con `Fill()`.
