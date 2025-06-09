using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public static class ASTPrinter
    {
        public static void Print(ISemanticNode node)
        {
            PrettyPrint(node, "", true);
        }

        private static void PrettyPrint(object? node, string indent, bool isLast)
        {
            if (node == null)
                return;

            var marker = isLast ? "└── " : "├── ";
            Console.Write(indent);
            Console.Write(marker);
            Console.WriteLine(NodeLabel(node));

            indent += isLast ? "    " : "│   ";

            if (node is ProgramNode prog)
            {
                for (int i = 0; i < prog.Instructions.Count; i++)
                    PrettyPrint(prog.Instructions[i], indent, i == prog.Instructions.Count - 1);
            }
            else if (node is SpawnNode spawn)
            {
                PrettyPrintLabeled("X", spawn.XExpression, indent, false);
                PrettyPrintLabeled("Y", spawn.YExpression, indent, true);
            }
            else if (node is AssignmentNode assign)
            {
                PrettyPrintLabeled($"Identifier: {assign.Identifier}", assign.Expression, indent, true);
            }
            else if (node is LabelNode lbl)
            {
                PrettyPrintLabeled($"Label: {lbl.LabelName}", null, indent, true);
            }
            else if (node is GoToNode go)
            {
                PrettyPrintLabeled($"Label: {go.Label}", go.Condition, indent, true);
            }
            else if (node is SizeNode sz)
            {
                PrettyPrintLabeled("Size", sz.SizeExpression, indent, true);
            }
            else if (node is ColorNode clr)
            {
                PrettyPrintLabeled($"Color: {clr.Color}", null, indent, true);
            }
            else if (node is DrawLineNode dl)
            {
                PrettyPrintLabeled("dirX", dl.DirXExpression, indent, false);
                PrettyPrintLabeled("dirY", dl.DirYExpression, indent, false);
                PrettyPrintLabeled("distance", dl.DistanceExpression, indent, true);
            }
            else if (node is DrawCircleNode dc)
            {
                PrettyPrintLabeled("dirX", dc.DirXExpression, indent, false);
                PrettyPrintLabeled("dirY", dc.DirYExpression, indent, false);
                PrettyPrintLabeled("radius", dc.RadiusExpression, indent, true);
            }
            else if (node is DrawRectangleNode dr)
            {
                PrettyPrintLabeled("dirX", dr.DirXExpression, indent, false);
                PrettyPrintLabeled("dirY", dr.DirYExpression, indent, false);
                PrettyPrintLabeled("distance", dr.DistanceExpression, indent, false);
                PrettyPrintLabeled("width", dr.WidthExpression, indent, false);
                PrettyPrintLabeled("height", dr.HeightExpression, indent, true);
            }
            else if (node is ExpressionNode expr)
            {
                // Recursivo para nodos binarios, unarios o funciones
                switch (expr)
                {
                    case LiteralNode lit:
                        PrettyPrintLabeled($"Literal({lit.Value})", null, indent, true);
                        break;

                    case VariableNode v:
                        PrettyPrintLabeled($"Variable({v.Name})", null, indent, true);
                        break;

                    case UnaryExpressionNode un:
                        PrettyPrintLabeled($"UnaryOp({un.Operator})", un.Operator, indent, true);
                        break;

                    case BinaryExpressionNode bin:
                        PrettyPrintLabeled($"BinaryOp({bin.Operator})", bin.LeftExpressionNode, indent, false);
                        PrettyPrint(bin.RightExpressionNode, indent, true);
                        break;

                    case BuiltInFunctionNode fn:
                        for (int i = 0; i < fn.Arguments.Count; i++)
                            PrettyPrintLabeled($"{fn.FunctionKind} Arg{i + 1}", fn.Arguments[i], indent, i == fn.Arguments.Count - 1);
                        break;

                    case InvalidExpressionNode inv:
                        PrettyPrintLabeled($"Invalid({inv.Why})", null, indent, true);
                        break;
                }
            }
        }

        private static void PrettyPrintLabeled(string label, object? node, string indent, bool isLast)
        {
            Console.Write(indent);
            Console.Write(isLast ? "└── " : "├── ");
            Console.WriteLine($"{label}:");
            PrettyPrint(node, indent + (isLast ? "    " : "│   "), true);
        }

        private static string NodeLabel(object node)
        {
            return node switch
            {
                ProgramNode => "ProgramNode",
                SpawnNode => "SpawnNode",
                AssignmentNode => "AssignmentNode",
                LabelNode => "LabelNode",
                GoToNode => "GoToNode",
                SizeNode => "SizeNode",
                ColorNode => "ColorNode",
                DrawLineNode => "DrawLineNode",
                DrawCircleNode => "DrawCircleNode",
                DrawRectangleNode => "DrawRectangleNode",
                FillNode => "FillNode",
                _ => node.GetType().Name
            };
        }
    }
}