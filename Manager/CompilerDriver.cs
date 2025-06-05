// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Proyecto_Wall_E_Art;

// namespace Proyecto_Wall_E_Art
// {
//     public class CompilerDriver
//     {
//         public void Main()
//         {
//             string sourceCode = @"
//             Spawn(10,20)
//             Color(""Red"")
//             Size(2)
//             DrawLine(1,2,3)
//             Fill()
//             GoTo[label1](true)
//             label1:
//             ";

//             var lexer = new Lexer(sourceCode);

//             var tokens = lexer.LexAll();

//             var parser = new Parser(tokens);

//             var ast = parser.ParseProgram();

//             if (ErrorsCollecter.HasErrors())
//             {
//                 System.Console.WriteLine("Errores encontrados :");

//                 foreach (var item in ErrorsCollecter.GetErrors())
//                 {
//                     System.Console.WriteLine($"[Linea {item.Line}], {item.Message}");
//                 }
//             }

//             else
//             {
//                 System.Console.WriteLine("El AST se genero correctamente");

//             }
//         }

//         static void ImprimirAST(ProgramNode programNode, string indent = "")
//         {
//             foreach (var item in programNode.Instructions)
//             {
//                 System.Console.WriteLine(indent + item.GetType().Name);

//                 if (item is AssignmentNode a)
//                     System.Console.WriteLine((item + $"{a.Identifier} <- {a.Expression}"));

//                 if (item is DrawLineNode b)
//                     System.Console.WriteLine(item + $"{b.DirXExpression}, {b.DirYExpression}, {b.DistanceExpression}");

//                 if (item is ColorNode c)
//                 {
//                     System.Console.WriteLine(item + $" Color : {c.Color}");
//                 }
//             }
//         }
//     }
// }
