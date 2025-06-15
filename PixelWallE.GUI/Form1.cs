using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using Proyecto_Wall_E_Art;
using System.Text;

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

        /// <summary>
        /// Muestra en un MessageBox todos los errores de ErrorsCollecter
        /// filtrando por el tipo dado (ej: "LEXICAL", "SYNTAX") y con el título indicado.
        /// </summary>
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
            foreach (var kv in parser.labelsOfParse)
                semCtx.LabelsTable[kv.Key] = kv.Value;
            program.Validate(semCtx);

            if (semCtx.Errors.Any())
            {
                // Si prefieres usar el mismo helper, podrías volcar semCtx.Errors en ErrorsCollecter,
                // o escribir uno similar que lea semCtx.Errors. Aquí un ejemplo rápido:
                var texto = string.Join("\n", semCtx.Errors
                    .Select(err => $"[Línea {err.Line}] {err.Message}"));
                MessageBox.Show("ERRORES SEMÁNTICOS:\n" + texto,
                    "ERRORES SEMÁNTICOS",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            // // 1) Lectura del código
            // var source = rtbCode.Text;

            // // 2) Léxico
            // var lexer = new Lexer(source);
            // var tokens = lexer.LexAll().ToList();
            // if (ErrorsCollecter.GetErrors().Any(e => e.Type == "LEXICAL"))
            // {
            //     MessageBox.Show("Errores léxicos detectados.");
            //     return;
            // }

            // // 3) Parsing
            // var parser = new Parser(tokens);
            // var program = parser.ParseProgram();
            // if (ErrorsCollecter.GetErrors().Any(e => e.Type == "SYNTAX"))
            // {
            //     MessageBox.Show("Errores sintácticos detectados.");
            //     return;
            // }

            // // 4) Semántica
            // var semCtx = new SemanticContext();
            // foreach (var kv in parser.labelsOfParse)
            //     semCtx.LabelsTable[kv.Key] = kv.Value;
            // program.Validate(semCtx);
            // if (semCtx.Errors.Any())
            // {
            //     var msg = string.Join("\n", semCtx.Errors.Select(err => $"L{err.Line}: {err.Message}"));
            //     MessageBox.Show("Errores semánticos:\n" + msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //     return;
            // }

            // 5) Interpretación
            var interpreter = new Interpreter(program, canvasSize);
            interpreter.Start();

            // 6) Obtener Bitmap y mostrar
            var bmp = interpreter.ShowCanvasGrid(cellSize: 16);
            picCanvas.Image?.Dispose();
            picCanvas.Image = bmp;
        }
    }
}