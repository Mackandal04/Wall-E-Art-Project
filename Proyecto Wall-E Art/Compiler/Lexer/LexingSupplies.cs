using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public static class LexingSupplies
    {
        public static readonly Dictionary<string, SyntaxKind> kinds = new()
        {
            // Comandos principales
            ["Spawn"] = SyntaxKind.SpawnKeyword,
            ["Color"] = SyntaxKind.ColorKeyword,
            ["Size"] = SyntaxKind.SizeKeyword,
            ["DrawLine"] = SyntaxKind.DrawLineKeyword,
            ["DrawCircle"] = SyntaxKind.DrawCircleKeyword,
            ["DrawRectangle"] = SyntaxKind.DrawRectangleKeyword,
            ["Fill"] = SyntaxKind.FillKeyword,
            ["GoTo"] = SyntaxKind.GoToKeyword,

            // Funciones auxiliares
            ["GetActualX"] = SyntaxKind.GetActualXKeyword,
            ["GetActualY"] = SyntaxKind.GetActualYKeyword,
            ["GetCanvasSize"] = SyntaxKind.GetCanvasSizeKeyword,
            ["GetColorCount"] = SyntaxKind.GetColorCountKeyword,
            ["IsBrushColor"] = SyntaxKind.IsBrushColorKeyword,
            ["IsBrushSize"] = SyntaxKind.IsBrushSizeKeyword,
            ["IsCanvasColor"] = SyntaxKind.IsCanvasColorKeyword,

            //Bools
            ["true"] = SyntaxKind.TrueKeyword,
            ["false"] = SyntaxKind.FalseKeyword
        };

        public static SyntaxKind GetKeywordKind(string text)
        {
            return kinds.TryGetValue(text, out var kind) ? kind : SyntaxKind.IdentifierToken;
        }
    }
}