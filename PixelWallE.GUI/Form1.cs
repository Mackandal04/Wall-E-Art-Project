using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using Proyecto_Wall_E_Art;
using System.Text;
using System.Threading.Tasks;

namespace PixelWallE.GUI
{
    public partial class Form1 : Form
    {
        int canvasSize = 100;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog()
            {
                Filter = "WallE code (*.pw)|*.pw|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            rtbCode.Text = File.ReadAllText(dlg.FileName);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog()
            {
                Filter = "WallE code (*.pw)|*.pw|All files (*.*)|*.*",
                FileName = "input.pw"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            File.WriteAllText(dlg.FileName, rtbCode.Text);
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            canvasSize = (int)nudCanvasSize.Value;
            // Crea un Bitmap en blanco del nuevo tamaño
            var bmp = new Bitmap(canvasSize, canvasSize);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // Muestra inmediatamente el canvas vacío
            picCanvas.Image?.Dispose();
            picCanvas.Image = bmp;
        }

        private bool ShowErrors(string tipo, string titulo)
        {
            // Filtramos los errores de ErrorsCollecter de este tipo
            var errs = ErrorsCollecter.GetErrors()
                        .Where(err => err.Type == tipo)
                        .Select(err => $"[Línea {err.Line}] {err.Message}")
                        .ToList();

            if (!errs.Any())
                return false;

            // Construimos el mensaje completo
            var texto = new StringBuilder();
            texto.AppendLine($"{titulo}:");
            foreach (var e in errs)
                texto.AppendLine(e);

            // Lo mostramos
            MessageBox.Show(texto.ToString(), titulo,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            return true;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            // 1) LEXER
            var lexer = new Lexer(rtbCode.Text);
            var tokens = lexer.LexAll().ToList();

            // Si hay errores léxicos, los mostramos y salimos
            if (ShowErrors("LEXICAL", "ERRORES LÉXICOS DETECTADOS"))
            {
                ErrorsCollecter.ErrorsClear();
                return;
            }

            // 2) PARSING
            ErrorsCollecter.ErrorsClear();
            var parser = new Parser(tokens);
            var program = parser.ParseProgram();

            // Si hay errores sintácticos, los mostramos y salimos
            if (ShowErrors("SYNTAX", "ERRORES SINTÁCTICOS"))
            {
                ErrorsCollecter.ErrorsClear();
                return;
            }

            // 3) SEMÁNTICA
            var semCtx = new SemanticContext();
            foreach (var labelKey in parser.labelsOfParse)
            {
                if (semCtx.LabelsTable.ContainsKey(labelKey.Key))
                    semCtx.GetErrors($"Etiqueta duplicada '{labelKey.Key}'", labelKey.Value);
                else
                    semCtx.LabelsTable.Add(labelKey.Key, labelKey.Value);
            }
            program.Validate(semCtx);

            if (semCtx.Errors.Any())
            {
                var texto = string.Join("\n", semCtx.Errors
                    .Select(err => $"[Línea {err.Line}] {err.Message}"));
                MessageBox.Show("ERRORES SEMÁNTICOS:\n" + texto,
                    "ERRORES SEMÁNTICOS",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Parámetro de escalado
            int cellSize = 16;

            // 1) Crear bitmap escalado y dibujar cuadricula
            int pixelCount = canvasSize;                      // número lógico de píxeles
            var bmp = new Bitmap(pixelCount * cellSize,
                                 pixelCount * cellSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.Clear(Color.White);

                // Dibuja líneas de cuadricula muy suaves
                using var pen = new Pen(Color.FromArgb(50, Color.Gray));
                // verticales
                for (int x = 0; x <= pixelCount; x++)
                    g.DrawLine(pen, x * cellSize, 0,
                                   x * cellSize, pixelCount * cellSize);
                // horizontales
                for (int y = 0; y <= pixelCount; y++)
                    g.DrawLine(pen, 0, y * cellSize,
                                   pixelCount * cellSize, y * cellSize);
            }

            // Asigna el bitmap al PictureBox
            picCanvas.Image?.Dispose();
            picCanvas.Image = bmp;

            // 2) Crear intérprete y suscribirse al evento PixelDrawn
            var interpreter = new Interpreter(program, canvasSize);
            interpreter.PixelDrawn += (px, py) =>
            {
                // calculamos la región en el bitmap donde pintaremos
                int rx = px * cellSize;
                int ry = py * cellSize;

                // encolar en UI thread
                picCanvas.BeginInvoke((Action)(() =>
                {
                    using var g2 = Graphics.FromImage(picCanvas.Image);
                    g2.InterpolationMode = InterpolationMode.NearestNeighbor;
                    // rellena el bloque cellSize×cellSize
                    var color = ColorTranslator.FromHtml(interpreter.currentColor);
                    g2.FillRectangle(
                        new SolidBrush(color),
                        rx, ry, cellSize, cellSize);
                    picCanvas.Refresh();
                }));
            };
            Task.Run(() => interpreter.Start());
        }
    }
}