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
    }
}