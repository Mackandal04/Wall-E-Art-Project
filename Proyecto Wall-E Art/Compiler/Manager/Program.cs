using System;
using System.IO;
using System.Collections.Generic;
using Proyecto_Wall_E_Art;

class CompilerRunner
{
    static void Main(string[] args)
    {
        var path = args.Length > 0 ? args[0] : "input.pw";

        if (!File.Exists(path))
        {
            System.Console.WriteLine($"Archivo {path} no encontrado");
            return;
        }

        var sourceText = File.ReadAllText(path);

        // while (true)
        // {
        //     Console.Clear();
        //     Console.WriteLine("===== Pixel Wall-E Compiler =====");
        //     Console.WriteLine("Escribe tu código (ENTER doble para ejecutar, 'salir' para terminar):");

        //     string source = LeerMultilinea();
        //     if (source.Trim().ToLower() == "salir") break;

        // 1. LEXER
        //System.Console.WriteLine("LEXER");
        var lexer = new Lexer(sourceText);
        var tokens = lexer.LexAll().ToList();

        if (ErrorsCollecter.HasErrors())
        {
            System.Console.WriteLine("ERRORES LEXICOS DETECTADOS");
            foreach (var item in ErrorsCollecter.GetErrors())
            {
                System.Console.WriteLine($"[Linea {item.Line}] {item.Message}");
            }

            return;
        }


        // Console.WriteLine("\n--- TOKENS ---");

        // foreach (var token in tokens)
        //     Console.WriteLine($"{token.Kind}  \"{token.Text}\"  (línea {token.Line})");

        // 2. PARSER
        // System.Console.WriteLine();
        // System.Console.WriteLine("PARSER");
        var parser = new Parser(tokens);
        //            System.Console.WriteLine($"Primer Token: {tokens.First().Kind} - \"{tokens.First().Text}\"");
        var program = parser.ParseProgram();


        //3. SEMANTIC
        //System.Console.WriteLine();

        var context = new SemanticContext();

        foreach (var label in parser.labelsOfParse)
        {
            if (context.LabelsTable.ContainsKey(label.Key))
                context.GetErrors($"Label {label.Key} duplicado", label.Value);

            else
                context.LabelsTable.Add(label.Key, label.Value);
        }
        program.Validate(context);

        if (context.Errors.Count > 0)
        {
            System.Console.WriteLine("ERRORES SEMÁNTICOS");

            foreach (var error in context.Errors)
                Console.WriteLine($"[Línea {error.Line}] {error.Message}");

            return;
        }

        // 4. ERRORES
        // System.Console.WriteLine();
        // System.Console.WriteLine("ERRORS");
        if (ErrorsCollecter.HasErrors())
        {
            Console.WriteLine("\n--- ERRORES SINTÁCTICOS ---");
            foreach (var error in ErrorsCollecter.GetErrors())
                Console.WriteLine($"[Línea {error.Line}] {error.Message}");
        }

        const int canvasSize = 100;
        var Interpreter = new Interpreter(program, canvasSize);
        Interpreter.Start();
        System.Console.WriteLine("Programa ejecutado, mira el resultado");
            // else
        // {
        //     // Console.WriteLine("\n--- AST GENERADO ---");
        //     // ImprimirAST(program);
        //     System.Console.WriteLine();
        //     System.Console.WriteLine(" :) AST :");
        //     ASTPrinter.Print(program);
        // }

        // Console.WriteLine("\nPresiona una tecla para volver a escribir código...");
        // Console.ReadKey();
    }
}

    // static string LeerMultilinea()
    // {
    //     string line;
    //     var lines = new List<string>();
    //     while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()!))
    //         lines.Add(line);
    //     return string.Join("\n", lines);
    // }

    // static void ImprimirAST(ProgramNode program, string indent = "")
    // {
    //     foreach (var instr in program.Instructions)
    //     {
    //         switch (instr)
    //         {
    //             case SpawnNode s:
    //                 Console.WriteLine($"{indent}Spawn({FormatearExpr(s.XExpression)}, {FormatearExpr(s.YExpression)})");
    //                 break;
    //             case ColorNode c:
    //                 Console.WriteLine($"{indent}Color(\"{c.Color}\")");
    //                 break;
    //             case SizeNode sz:
    //                 Console.WriteLine($"{indent}Size({FormatearExpr(sz.SizeExpression)})");
    //                 break;
    //             case DrawLineNode d:
    //                 Console.WriteLine($"{indent}DrawLine({FormatearExpr(d.DirXExpression)}, {FormatearExpr(d.DirYExpression)}, {FormatearExpr(d.DistanceExpression)})");
    //                 break;
    //             case DrawCircleNode dc:
    //                 Console.WriteLine($"{indent}DrawCircle({FormatearExpr(dc.DirXExpression)}, {FormatearExpr(dc.DirYExpression)}, {FormatearExpr(dc.RadiusExpression)})");
    //                 break;
    //             case DrawRectangleNode dr:
    //                 Console.WriteLine($"{indent}DrawRectangle({FormatearExpr(dr.DirXExpression)}, {FormatearExpr(dr.DirYExpression)}, {FormatearExpr(dr.DistanceExpression)}, {FormatearExpr(dr.WidthExpression)}, {FormatearExpr(dr.HeightExpression)})");
    //                 break;
    //             case FillNode f:
    //                 Console.WriteLine($"{indent}Fill()");
    //                 break;
    //             case AssignmentNode a:
    //                 Console.WriteLine($"{indent}{a.Identifier} <- {FormatearExpr(a.Expression)}");
    //                 break;
    //             case GoToNode g:
    //                 Console.WriteLine($"{indent}GoTo[{g.Label}]({FormatearExpr(g.Condition)})");
    //                 break;
    //             case LabelNode l:
    //                 Console.WriteLine($"{indent}{l.LabelName}:");
    //                 break;
    //             default:
    //                 Console.WriteLine($"{indent}{instr.GetType().Name} (no soportado para impresión)");
    //                 break;
    //         }
    //     }
    // }

//     static string FormatearExpr(ExpressionNode expr)
//     {
//         return expr switch
//         {
//             LiteralNode l      => l.Value?.ToString() ?? "null",
//             VariableNode v     => v.Name,
//             UnaryExpressionNode u => $"{u.Operator} {FormatearExpr(u.MiddleExpression)}",
//             BinaryExpressionNode b => $"({FormatearExpr(b.LeftExpressionNode)} {b.Operator} {FormatearExpr(b.RightExpressionNode)})",
//             BuiltInFunctionNode f => $"{f.FunctionKind}({string.Join(", ", f.Arguments.ConvertAll(FormatearExpr))})",
//             InvalidExpressionNode inv => $"[Error: {inv.Why}]",
//             _ => expr.GetType().Name
//         };
//     }
// }