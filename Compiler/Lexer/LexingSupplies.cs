using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public class LexingSupplies
    {
        public readonly Dictionary<string, SyntaxKind> KeywordKind = new()
        {
            //Comandos
            ["Spawn"] = SyntaxKind.SpawnToken,
            ["Color"] = SyntaxKind.ColorToken,
            ["Size"] = SyntaxKind.SizeToken,
            ["DrawLine"] = SyntaxKind.DrawLineToken,
            ["DrawnCircle"] = SyntaxKind.DrawCircleToken,
            ["DrawRectangle"] = SyntaxKind.DrawRectangleToken,
            ["Fill"] = SyntaxKind.FillToken,
            ["GoTo"] = SyntaxKind.GoToToken,

            //Functions
            ["GetActualX"] = SyntaxKind.GetActualXToken,
            ["GetActualY"] = SyntaxKind.GetActualYToken,
            ["GetCanvasSize"] = SyntaxKind.GetCanvasSizeToken,
            ["GetColorCount"] = SyntaxKind.GetColorCountToken,
            ["IsBrushColor"] = SyntaxKind.IsBrushColorToken,
            ["IsBrushSize"] = SyntaxKind.IsBrushSizeToken,
            ["IsCanvasColor"] = SyntaxKind.IsCanvasColorToken,

            //Colors
            ["Red"] = SyntaxKind.RedKeyword,
            ["Blue"] = SyntaxKind.BlueKeyword,
            ["Green"] = SyntaxKind.GreenKeyword,
            ["Yellow"] = SyntaxKind.YellowKeyword,
            ["Orange"] = SyntaxKind.OrangeKeyword,
            ["Purple"] = SyntaxKind.PurpleKeyword,
            ["Black"] = SyntaxKind.BlackKeyword,
            ["White"] = SyntaxKind.WhiteKeyword,
            ["Transparent"] = SyntaxKind.TransparentKeyword
        };

        public readonly Dictionary<char,Func<int, int, char, (SyntaxToken, int)>> LexMathCharacters = new()
        {
            ['+'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.PlusToken, line, pos,"+", null!), pos +1),
            ['-'] = LexMinusOrArrowChar,
            ['*'] = LexMultOrPowChar,
            ['/'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.DivisionToken, line, pos,"/", null!), pos +1),
            ['%'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.ModToken, line, pos,"%", null!), pos +1),
            ['('] = (pos,line, _) => (new SyntaxToken(SyntaxKind.OpenParenthesisToken, line, pos,"(", null!), pos +1),
            [')'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.CloseParenthesisToken, line, pos,")", null!), pos +1),
            [','] = (pos,line, _) => (new SyntaxToken(SyntaxKind.ComaToken, line, pos,",", null!), pos +1),
            ['>'] = LexGreaterThanChar,
            ['<'] = LexLessThanChar,
            ['='] = LexEqualsChar,
            ['!'] = LexNotEqualChar,
            ['&'] = LexAndsChar,
            ['|'] = LexOrsChar
        };


        public SyntaxKind GetKeywordKind(string token)
        {
            if(KeywordKind.ContainsKey(token))
                return KeywordKind[token];

            return SyntaxKind.IdentifierToken;
        }
        static(SyntaxToken, int) LexMinusOrArrowChar(int pos , int line, char next)
        {
            if(next == '>')
                return (new SyntaxToken( SyntaxKind.AssignmentToken, line, pos, "->", null!), pos +2);
            
            return(new SyntaxToken(SyntaxKind.MinusToken, line, pos, "-", null!), pos+1);
        }
        static(SyntaxToken, int) LexMultOrPowChar(int pos, int line, char next)
        {
            if(next == '*')
                return (new SyntaxToken(SyntaxKind.PowToken, line, pos, "**" , null!), pos+2);
            
            return (new SyntaxToken(SyntaxKind.MultToken, line, pos, "*", null!), pos+1);
        }

        static (SyntaxToken,int) LexGreaterThanChar(int pos, int line, char next)
        {
            if(next == '=')
                return (new SyntaxToken(SyntaxKind.GreaterOrEquealToken, line,pos, ">=", null!), pos +2);

            return (new SyntaxToken(SyntaxKind.GreaterToken, line, pos , ">", null!), pos +1);
        }

        static (SyntaxToken, int) LexLessThanChar(int pos , int line, char next)
        {
            if(next == '=')
                return (new SyntaxToken(SyntaxKind.LessOrEqualToken, line, pos, "<=", null!), pos +2);
            
            return(new SyntaxToken(SyntaxKind.LessToken, line, pos, "<", null!), pos +1);
        }

        static (SyntaxToken, int) LexEqualsChar(int pos, int line, char next)
        {
            if(next == '=')
                return (new SyntaxToken(SyntaxKind.EqualToken, line, pos , "==", null!), pos+2);

            throw new Exception("Caracter inesperado en la linea " + line + " posicion " + pos);
        }

        static (SyntaxToken, int) LexNotEqualChar(int pos, int line, char next)
        {
            if(next == '=')
                return (new SyntaxToken(SyntaxKind.NotEqualToken, line, pos, "!=" , null!), pos +2);

            throw new Exception("Caracter inesperado en la linea " + line + " posicion " + pos);
        }

        static(SyntaxToken, int) LexAndsChar(int pos, int line, char next)
        {
            if(next == '&')
                return (new SyntaxToken(SyntaxKind.AndAndToken,line, pos, "&&", null!), pos +2);

            throw new Exception ("Caracter inespereado en la linea " + line + " posicion " + pos);
        }
        
        static (SyntaxToken, int) LexOrsChar(int pos, int line, char next)
        {
            if(next == '|')
                return (new SyntaxToken(SyntaxKind.OrOrToken, line, pos, "||", null!), pos +2);
            
            throw new Exception("Caracter inesperado en la linea" + line + " posicion " + pos);
        }
    }
}