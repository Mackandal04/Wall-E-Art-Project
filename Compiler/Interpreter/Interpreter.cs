using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;
using System.Drawing;
using System.Drawing.Imaging;

namespace Proyecto_Wall_E_Art
{
    public class Interpreter
    {
        int walleX = 0;
        int walleY = 0;
        string currentColor = "Transparent";
        int brushSize = 1;

        //guarda int o bool
        readonly Dictionary<string, object> VariablesTable = new Dictionary<string, object>();

        readonly Dictionary<string, int> LabelsTable = new Dictionary<string, int>();

        // matriz de colores que representa el canvas
        readonly Color[,] canvas;

        readonly ProgramNode programNode;

        public Interpreter(ProgramNode programNode, int canvasSize)
        {
            this.programNode = programNode;

            canvas = new Color[canvasSize, canvasSize];

            for (int i = 0; i < canvas.GetLength(0); i++)
            {
                for (int j = 0; j < canvas.GetLength(1); j++)
                {
                    canvas[i, j] = Color.White;
                }
            }
        }

        void LablesTableCollecter()
        {
            for (int i = 0; i < programNode.Instructions.Count; i++)
            {
                if (programNode.Instructions[i] is LabelNode labelNode)
                {
                    if (LabelsTable.ContainsKey(labelNode.LabelName))
                        throw new Exception($"Label duplicado '{labelNode.LabelName}' en la línea {labelNode.Line}");

                    // +1 porque el label no cuenta como instrucción
                    LabelsTable[labelNode.LabelName] = i + 1;

                }
            }
        }

        public void Start()
        {
            LablesTableCollecter();

            int flag = 0; // Puntero de comandos

            while (flag < programNode.Instructions.Count)
            {
                if (programNode.Instructions[flag] is GoToNode goToNode)
                {
                    if (EvaluateBool(goToNode.Condition) && LabelsTable.ContainsKey(goToNode.Label))
                    {
                        flag = LabelsTable[goToNode.Label];
                    }

                    else
                        flag++;
                }

                DoInstruccion(programNode.Instructions[flag]);
                flag++;
            }

            ShowCanvas("output.png");
        }

        void DoInstruccion(InstructionNode instructionNode)
        {
            if (instructionNode is SpawnNode spawnNode)
                DoSpawn(spawnNode);

            else if (instructionNode is ColorNode colorNode)
                DoColor(colorNode);

            else if (instructionNode is SizeNode sizeNode)
                DoSize(sizeNode);

            else if (instructionNode is DrawLineNode drawLineNode)
                DoDrawLine(drawLineNode);

            //else if (instructionNode is DrawCircleNode drawCircleNode)
                //DoDrawCircle(drawCircleNode);

            else if (instructionNode is DrawRectangleNode drawRectangleNode)
                DoDrawRectangle(drawRectangleNode);

            else if (instructionNode is FillNode fillNode)
                DoFill(fillNode);

            else if (instructionNode is AssignmentNode assignmentNode)
                DoAssignment(assignmentNode);

            else if (instructionNode is LabelNode labelNode)
                DoLabel(labelNode);

            else
                throw new Exception($"Instrucción no reconocida: {instructionNode.GetType().Name} en la línea {instructionNode.Line}");
        }

        private void DoGoTo(GoToNode goToNode)
        {
            throw new NotImplementedException();
        }

        private void DoLabel(LabelNode labelNode)
        {
            throw new NotImplementedException();
        }

        private void DoAssignment(AssignmentNode assignmentNode)
        {
            int value = EvaluateInt(assignmentNode.Expression);
            VariablesTable[assignmentNode.Identifier] = value;
        }

        private void DoFill(FillNode fillNode)
        {
            var targetColor = canvas[walleX, walleY];

            //var newcolor = ColorFromName(currentColor);
            //if (targetColor == newcolor) return;
            //FloodFill(walleX, walleY, targetColor, newcolor);
        }

        private void DoDrawRectangle(DrawRectangleNode drawRectangleNode)
        {
            throw new NotImplementedException();
        }

        private void ExecuteDrawCircle(DrawCircleNode drawCircleNode)
        {
            int dx = EvaluateInt(drawCircleNode.DirXExpression);
            int dy = EvaluateInt(drawCircleNode.DirYExpression);
            int r = EvaluateInt(drawCircleNode.RadiusExpression);

            // Centro del círculo
            int cCenterX = walleX + dx * r;
            int cCenterY = walleY + dy * r;

            for (int i = -r; i <= r; i++)
            {
                int y0 = (int)Math.Round(Math.Sqrt(r * r - i * i));
                // PlotCirclePoints(cCenterX, cCenterY, i, y0); // Parte superior
                // PlotCirclePoints(cCenterY, cCenterY, i, -y0); // Parte inferior
            }

            walleX = cCenterX;
            walleY = cCenterY;
        }

        private void DoDrawLine(DrawLineNode drawLineNode)
        {
            int dirX = EvaluateInt(drawLineNode.DirXExpression);

            int dirY = EvaluateInt(drawLineNode.DirYExpression);

            int dist = EvaluateInt(drawLineNode.DistanceExpression);

            for (int i = 0; i <= dist; i++)
            {
                int paintX = walleX + (dirX * i);

                int paintY = walleY + (dirY * i);

                Draw(paintX, paintY);
            }

            walleX += dirX * dist;

            walleY += dirY * dist;
        }

        private void Draw(int paintX, int paintY)
        {
            int size = canvas.GetLength(0);
            int r = brushSize / 2;
            for (int ox = -r; ox <= r; ox++)
                for (int oy = -r; oy <= r; oy++)
                {
                    int tx = paintX + ox, ty = paintY + oy;
                    if (tx >= 0 && tx < size && ty >= 0 && ty < size)
                        canvas[tx, ty] = ColorFromName(currentColor);
                }
        }

        private Color ColorFromName(string name) =>
            name switch
            {
                "Red"         => Color.Red,
                "Blue"        => Color.Blue,
                "Green"       => Color.Green,
                "Yellow"      => Color.Yellow,
                "Orange"      => Color.Orange,
                "Purple"      => Color.Purple,
                "Black"       => Color.Black,
                "White"       => Color.White,
                "Transparent" => Color.Transparent,
                _             => Color.Black
            };

        private void DoSize(SizeNode sizeNode)
        {
            int tempSize = EvaluateInt(sizeNode.SizeExpression);

            if (tempSize % 2 == 2)
                brushSize = tempSize - 1;

            else
                brushSize = tempSize;

            if (brushSize < 1)
                brushSize = 1;
        }

        private void DoColor(ColorNode colorNode)
        {
            currentColor = colorNode.Color;
        }

        private void DoSpawn(SpawnNode spawnNode)
        {
            walleX = EvaluateInt(spawnNode.XExpression);
            walleY = EvaluateInt(spawnNode.YExpression);

            if (walleX < 0 || walleY < 0 || walleX > canvas.GetLength(0) || walleY > canvas.GetLength(1))
                throw new Exception($"Sapwneo no valido, Wall-E fuera de rango");
        }

        object Evaluate(ExpressionNode expressionNode)
        {
            if (expressionNode is LiteralNode literalNode)
                return literalNode.Value;

            else if (expressionNode is VariableNode variableNode)
                return VariablesTable[variableNode.Name];

            else if (expressionNode is UnaryExpressionNode unaryExpressionNode)
            {
                var val = Evaluate(unaryExpressionNode.MiddleExpression);
                return unaryExpressionNode.Operator == UnaryOperator.Minus ? -(int)val : !(bool)val;
            }

            if (expressionNode is BinaryExpressionNode binaryExpressionNode)
            {
                var left = Evaluate(binaryExpressionNode.LeftExpressionNode);

                var right = Evaluate(binaryExpressionNode.RightExpressionNode);

                if (binaryExpressionNode.Operator == BinaryOperator.Plus)

                    return (int)left + (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.Minus)
                    return (int)left - (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.Mult)
                    return (int)left * (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.Pow)
                    return Math.Pow((int)left, (int)right);

                else if (binaryExpressionNode.Operator == BinaryOperator.Slash)
                    return (int)left / (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.Mod)
                    return (int)left % (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.LessThan)
                    return (int)left < (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.GreaterThan)
                    return (int)left > (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.LessThanOrEqual)
                    return (int)left <= (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.GreaterThanOrEqual)
                    return (int)left >= (int)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.Equal)
                    return left.Equals(right);

                else if (binaryExpressionNode.Operator == BinaryOperator.NotEqual)
                    return !left.Equals(right);

                else if (binaryExpressionNode.Operator == BinaryOperator.AndAnd)
                    return (bool)left && (bool)right;

                else if (binaryExpressionNode.Operator == BinaryOperator.OrOr)
                    return (bool)left || (bool)right;
            }

            if (expressionNode is BuiltInFunctionNode builtInFunctionNode)
            {
                if (builtInFunctionNode.FunctionKind is FunctionKind.GetActualX)
                    return walleX;

                else if (builtInFunctionNode.FunctionKind is FunctionKind.GetActualY)
                    return walleY;

                else if (builtInFunctionNode.FunctionKind is FunctionKind.GetCanvasSize)
                    return canvas.GetLength(0);

                else if (builtInFunctionNode.FunctionKind is FunctionKind.IsBrushColor)
                {
                    var color = (string)Evaluate(builtInFunctionNode.Arguments[0]);
                    return currentColor == color;
                }

                else if (builtInFunctionNode.FunctionKind is FunctionKind.IsBrushSize)
                    return brushSize == (int)Evaluate(builtInFunctionNode.Arguments[0]);

                else if (builtInFunctionNode.FunctionKind is FunctionKind.GetColorCount)
                {
                    var colorName = (string)Evaluate(builtInFunctionNode.Arguments[0]);
                    int x1 = (int)Evaluate(builtInFunctionNode.Arguments[1]);
                    int y1 = (int)Evaluate(builtInFunctionNode.Arguments[2]);
                    int x2 = (int)Evaluate(builtInFunctionNode.Arguments[3]);
                    int y2 = (int)Evaluate(builtInFunctionNode.Arguments[4]);

                    // Si cualquier esquina fuera de rango, retorna 0
                    int size = canvas.GetLength(0);
                    if (x1 < 0 || x1 >= size || y1 < 0 || y1 >= size ||
                        x2 < 0 || x2 >= size || y2 < 0 || y2 >= size)
                        return 0;

                    int minX = Math.Min(x1, x2);
                    int maxX = Math.Max(x1, x2);
                    int minY = Math.Min(y1, y2);
                    int maxY = Math.Max(y1, y2);
                    int count = 0;

                    for (int ix = minX; ix <= maxX; ix++)
                        for (int iy = minY; iy <= maxY; iy++)
                            if (canvas[ix, iy].ToString() == colorName)
                                count++;

                    return count;
                }

                else if (builtInFunctionNode.FunctionKind is FunctionKind.IsCanvasColor)
                {
                    var colorName = (string)Evaluate(builtInFunctionNode.Arguments[0]);
                    int vertical = (int)Evaluate(builtInFunctionNode.Arguments[1]);
                    int horizontal = (int)Evaluate(builtInFunctionNode.Arguments[2]);

                    int tx = walleX + horizontal;
                    int ty = walleY + vertical;
                    int size = canvas.GetLength(0);

                    if (tx < 0 || tx >= size || ty < 0 || ty >= size)
                        return false;

                    return canvas[tx, ty].ToString() == colorName;

                }
            }

            throw new InvalidOperationException($"No se puede evaluar una expresion de tipo '{expressionNode.GetType().Name}' en la linea {expressionNode.Line}");
        }

        int EvaluateInt(ExpressionNode expressionNode)
        {
            return (int)Evaluate(expressionNode);
        }
        bool EvaluateBool(ExpressionNode expressionNode)
        {
            return (bool)Evaluate(expressionNode);
        }

        void ShowCanvas(string path)
        {
            int size = canvas.GetLength(0);
            using var bmp = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    bmp.SetPixel(i, j, canvas[i, j]);
            //bmp.SetPixel(10, 10, Color.Red);
            bmp.Save(path, ImageFormat.Png);

            // Después de bmp.Save(path, ImageFormat.Png);
            try
            {
                // En Windows, esto abrirá la imagen con la app por defecto
                var psi = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                // Opcionalmente, manejar Mac/Linux:
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", path);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", path);
            }
        }
    }
}