using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;
using Proyecto_Wall_E_Art;

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

        private void btnRun_Click(object sender, EventArgs e)
        {
            // 1) Lectura del código
            var source = rtbCode.Text;

            // 2) Léxico
            var lexer = new Lexer(source);
            var tokens = lexer.LexAll().ToList();
            if (ErrorsCollecter.GetErrors().Any(e => e.Type == "LEXICAL"))
            {
                MessageBox.Show("Errores léxicos detectados.");
                return;
            }

            // 3) Parsing
            var parser = new Parser(tokens);
            var program = parser.ParseProgram();
            if (ErrorsCollecter.GetErrors().Any(e => e.Type == "SYNTAX"))
            {
                MessageBox.Show("Errores sintácticos detectados.");
                return;
            }

            // 4) Semántica
            var semCtx = new SemanticContext();
            foreach (var kv in parser.labelsOfParse)
                semCtx.LabelsTable[kv.Key] = kv.Value;
            program.Validate(semCtx);
            if (semCtx.Errors.Any())
            {
                var msg = string.Join("\n", semCtx.Errors.Select(err => $"L{err.Line}: {err.Message}"));
                MessageBox.Show("Errores semánticos:\n" + msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 5) Interpretación
            var interpreter = new Interpreter(program, canvasSize);
            interpreter.Start();

            // 6) Obtener Bitmap y mostrar
            var bmp = interpreter.ShowCanvas();
            picCanvas.Image?.Dispose();
            picCanvas.Image = bmp;
        }
    }
}