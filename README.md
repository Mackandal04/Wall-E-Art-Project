# 🎨🤖 Pixel Wall‑E Art

![Banner](https://images2.alphacoders.com/111/111180.jpg) 

Pixel‑Wall-E Art , es un proyecto creativo que incluye un lenguaje minimalista que convierte tus scripts en dibujos de pixel‑art sobre un lienzo cuadriculado.
Controla a Wall‑E con comandos sencillos (Spawn, DrawLine, Fill, bucles y condicionales) y dibuja figuras, patrones y animaciones sin necesidad de GUI complejas.
Desarrollado en C# con Windows Forms y .NET, ideal para unir lógica de programación y arte digital.

---

## 🚀 Características Destacadas

| 📦 Componente        | ✔️ Funcionalidad Principal                                      | 🎯 Objetivo                                                            |
|----------------------|-----------------------------------------------------------------|------------------------------------------------------------------------|
| 🖋️ **Lenguaje**      | Sintaxis: `Spawn`, `Color`, `Size`, `DrawLine`, `DrawCircle`, `DrawRectangle`, `Fill`, asignaciones, etiquetas y saltos (`GoTo[...]`) | Permitir dibujar figuras y controlar flujo con variables y bucles      |
| 🔍 **Análisis**      | Lexer → Parser → AST → Semántico → Intérprete                   | Separar fases de compilación e interpretación para robustez y extensibilidad |
| 🖼️ **GUI WinForms**  | Editor de texto, botones Cargar/Guardar/Ejecutar, cuadricula, escalado nearest‑neighbor | Experiencia interactiva: ver tu código y resultado sin salir de la app  |

---

## 📜 Comandos y Sintaxis

| **Instrucción**                    | **Parámetros**                                    | **Efecto**                                                   |
|------------------------------------|---------------------------------------------------|--------------------------------------------------------------|
| `Spawn(x, y)`                      | `int x, int y`                                    | Inicializa a Wall‑E en `(x,y)`                                |
| `Color("name")`                    | `"Red"`, `"Blue"`, `"Green"`, `"Yellow"`, etc.    | Cambia color de brocha                                        |
| `Size(n)`                          | `n >= 1` (impar)                                   | Ajusta grosor de brocha (si es par lo redondea al inferior)   |
| `DrawLine(x, y, distance)`           | `x, y ∈ {-1,0,1}`, `distance ≥ 0`                   | Dibuja línea en dirección `(x, y)`                          |
| `DrawCircle(x, y, radius)`            | `x, y ∈ {-1,0,1}`, `radius ≥ 0`                      | Dibuja circunferencia de radio `radius`                            |
| `DrawRectangle(x, y, distance, width, heigh)`   | `x, y ∈ {-1,0,1}`, `d ≥ 0`, `width,heigh ≥ 1`            | Dibuja y rellena rectángulo centrado a distancia `distance`         |
| `Fill()`                           | —                                                 | Flood‑fill de región contigua                                 |
| `var <- Expression`                | literales, variables y operadores (`+ - * / % **`) | Asigna resultado de expresión                                 |
| `label` (línea sola)               | cadena alfanumérica                                | Define etiqueta para saltos                                   |
| `GoTo[label](condición)`           | `label`, `condición ∈ {bool}`                     | Salto condicional a etiqueta                                  |

---

## 🔧 Funciones Integradas

| Función                      | Parámetros & Retorno           | Descripción                                   |
|------------------------------|--------------------------------|-----------------------------------------------|
| `GetActualX()`               | () — → `int`                      | Retorna `X` actual de Wall‑E                  |
| `GetActualY()`               |() — → `int`                      | Retorna `Y` actual de Wall‑E                  |
| `GetCanvasSize()`            | () — → `int`                      | Retorna tamaño `n` (canvas `n×n`)             |
| `GetColorCount(color,x1,y1,x2,y2)` | `string,int,int,int,int` → `int` | Cuenta píxeles de `color` en sub‑región      |
| `IsBrushColor(color)`        | `string` → `0/1`               | Comprueba color de brocha                     |
| `IsBrushSize(size)`          | `int` → `0/1`                  | Comprueba grosor de brocha                    |
| `IsCanvasColor(color,v,h)`   | `string,int,int` → `0/1`       | Comprueba color de píxel relativo             |

---

## 📂 Estructura de Carpeta

```
Wall‑E-Art-Project/
├─ Proyecto_Wall_E_Art/       # Lógica (lexer, parser, AST, semántico, intérprete)
├─ PixelWallE(Ejecutable)/    # Ejecutable de facil acceso del proyecto en WindowsForm
├─ PixelWallE.GUI/            # WinForms GUI (Form1, Program.cs, PixelPictureBox)
└─ README.md                  # Documentación (este fichero)
```

---

## 🛠 Instalación y Uso

## ⚙️ Requisitos

- Sistema operativo:  
  – Windows

- .NET SDK & Runtime:
  – .NET Desktop Runtime para Windows (instalado junto al SDK o por separado)

- Herramientas recomendadas:  
  – VS Code con extensión C# o Visual Studio
  – Git (para clonar y gestionar el repositorio)

---

### 1. Aplicación Windows Forms

```bash
abra la terminal en la carpeta PixelWallE.GUI
dotnet run
Comienza el proyecto de windowsForm
```

1. Escribe o carga (`.pw`) tu script.
2. Ajusta tamaño del canvas
3. Pulsa **Ejecutar** para ver el resultado con cuadricula.

---

## 🧪 Casos de Prueba

Algunos ejemplos faciles de probar:

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
