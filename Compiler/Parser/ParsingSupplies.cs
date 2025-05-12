using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Proyecto_Wall_E_Art 
{ 
    public static class ParsingSupplies 
    {
        private static readonly Dictionary<SyntaxKind, int> binaryOperatorPrecedence = new() 
        {       
                [SyntaxKind.OrOrToken]             = 10,// ||  
                [SyntaxKind.AndAndToken]           = 20,// &&  
                [SyntaxKind.EqualToken]            = 30, // == 
                [SyntaxKind.LessToken]             = 40, // < 
                [SyntaxKind.GreaterToken]          = 40, // >
                [SyntaxKind.LessOrEqualToken]      = 40, // <= 
                [SyntaxKind.GreaterOrEqualToken]   = 40, // >= 
                [SyntaxKind.PlusToken]             = 50, // + 
                [SyntaxKind.MinusToken]            = 50, // - 
                [SyntaxKind.MultToken]             = 60, // * 
                [SyntaxKind.SlashToken]            = 60, // / 
                [SyntaxKind.ModToken]              = 60, // % 
                [SyntaxKind.PowToken]              = 70  // **
        };

        private static readonly Dictionary<SyntaxKind, int> unaryOperatorPrecedence = new()
        {
            [SyntaxKind.PlusToken]  = 80,
            [SyntaxKind.MinusToken] = 80
        };

        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            return binaryOperatorPrecedence.TryGetValue(kind, out int value) ? value : 0 ;
        }

        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            return unaryOperatorPrecedence.TryGetValue(kind, out int value) ? value : 0 ;
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            return text switch
            {
                "Spawn"          => SyntaxKind.SpawnKeyword,
                "Color"          => SyntaxKind.ColorKeyword,
                "Size"           => SyntaxKind.SizeKeyword,
                "DrawLine"       => SyntaxKind.DrawLineKeyword,
                "DrawCircle"     => SyntaxKind.DrawCircleKeyword,
                "DrawRectangle"  => SyntaxKind.DrawRectangleKeyword,
                "Fill"           => SyntaxKind.FillKeyword,
                "GoTo"           => SyntaxKind.GoToKeyword,
                "GetActualX"     => SyntaxKind.GetActualXKeyword,
                "GetActualY"     => SyntaxKind.GetActualYKeyword,
                "GetCanvasSize"  => SyntaxKind.GetCanvasSizeKeyword,
                "GetColorCount"  => SyntaxKind.GetColorCountKeyword,
                "IsBrushColor"   => SyntaxKind.IsBrushColorKeyword,
                "IsBrushSize"    => SyntaxKind.IsBrushSizeKeyword,
                "IsCanvasColor"  => SyntaxKind.IsCanvasColorKeyword,
                "true"           => SyntaxKind.TrueKeyword,
                "false"          => SyntaxKind.FalseKeyword,
                _                 => SyntaxKind.IdentifierToken
            };
        }

        public static SyntaxKind GetOperatorKind(string op)
        {
            return op switch
            {
                "==" => SyntaxKind.EqualToken,
                "<=" => SyntaxKind.LessOrEqualToken,
                ">=" => SyntaxKind.GreaterOrEqualToken,
                "&&" => SyntaxKind.AndAndToken,
                "||" => SyntaxKind.OrOrToken,
                _     => SyntaxKind.ErrorToken
            };
        }

        public static string BackSlashEval(string text, int line)
        {
            char[] scapes = { 'n', 'r', 't', 'a', 'f', 'b', 'v', '"', '\'', '\\' };
            
            char[] scapeSequency = { '\n', '\r', '\t', '\a', '\f', '\b', '\v' };

            int backSlashIndex = text.IndexOf("\\");

            while (backSlashIndex != -1)
            {
                int count = 0;
                
                for (int i = backSlashIndex; i < text.Length && text[i] == '\\'; i++)
                    count++;

                text = text.Remove(backSlashIndex, count / 2);

                if (count % 2 != 0)
                {
                    int scapeIndex = Array.IndexOf(scapes, text[backSlashIndex + count - count / 2]);
                    text = text.Remove(backSlashIndex, 1);

                    if (scapeIndex < 0)
                    {
                        Error.SetError("SYNTAX", $"Line '{line}' : Invalid character in string");
                        return "";
                    }
                    
                    if (!(scapes[scapeIndex] == '"' || scapes[scapeIndex] == '\'' ||  scapes[scapeIndex] == '\\'))
                    {
                        text = text.Remove(backSlashIndex + count - count / 2 - 1, 1);
                        text = text.Insert(backSlashIndex + count - count / 2 - 1, scapeSequency[scapeIndex].ToString());
                    }
                }

                backSlashIndex = text.IndexOf("\\", backSlashIndex + 1);
            }

            return text;
        }
    }
}