using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public class LexingSupplies
    {
        public static readonly Dictionary<string, SyntaxKind> KeywordKind = new()
        {
            //Comands
            ["Spawn"] = SyntaxKind.SpawnKeyword,
            ["Color"] = SyntaxKind.ColorKeyword,
            ["Size"] = SyntaxKind.SizeKeyword,
            ["DrawLine"] = SyntaxKind.DrawLineKeyword,
            ["DrawCircle"] = SyntaxKind.DrawCircleKeyword,
            ["DrawRectangle"] = SyntaxKind.DrawRectangleKeyword,
            ["Fill"] = SyntaxKind.FillKeyword,
            ["GoTo"] = SyntaxKind.GoToKeyword,

            //Functions
            ["GetActualX"] = SyntaxKind.GetActualXKeyword,
            ["GetActualY"] = SyntaxKind.GetActualYKeyword,
            ["GetCanvasSize"] = SyntaxKind.GetCanvasSizeKeyword,
            ["GetColorCount"] = SyntaxKind.GetColorCountKeyword,
            ["IsBrushColor"] = SyntaxKind.IsBrushColorKeyword,
            ["IsBrushSize"] = SyntaxKind.IsBrushSizeKeyword,
            ["IsCanvasColor"] = SyntaxKind.IsCanvasColorKeyword,
        };

        public static readonly Dictionary<char,Func<int, int, char, (SyntaxToken, int)>> LexMathCharacters = new()
        {
            ['+'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.PlusToken, line, pos,"+", null!), pos +1),
            ['-'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.MinusToken ,line, pos,"-", null!), pos +1),
            ['*'] = LexMultOrPowChar,
            ['/'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.SlashToken, line, pos,"/", null!), pos +1),
            ['%'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.ModToken, line, pos,"%", null!), pos +1),
            ['('] = (pos,line, _) => (new SyntaxToken(SyntaxKind.OpenParenthesisToken, line, pos,"(", null!), pos +1),
            [')'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.CloseParenthesisToken, line, pos,")", null!), pos +1),
            ['['] = (pos,line, _) => (new SyntaxToken(SyntaxKind.OpenBracketToken, line, pos,"[", null!), pos +1),
            [']'] = (pos,line, _) => (new SyntaxToken(SyntaxKind.CloseBracketToken, line, pos,"]", null!), pos +1),
            [','] = (pos,line, _) => (new SyntaxToken(SyntaxKind.CommaToken, line, pos,",", null!), pos +1),
            ['>'] = LexGreaterThanChar,
            ['<'] = LexLessOrAssignmentChar,
            ['='] = LexEqualsChar,
            ['&'] = LexAndsChar,
            ['|'] = LexOrsChar
        };


        public static SyntaxKind GetKeywordKind(string token)
        {
            if (KeywordKind.TryGetValue(token, out SyntaxKind value))
                return value;

            return SyntaxKind.IdentifierToken;
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
                return (new SyntaxToken(SyntaxKind.GreaterOrEqualToken, line,pos, ">=", null!), pos +2);

            return (new SyntaxToken(SyntaxKind.GreaterToken, line, pos , ">", null!), pos +1);
        }

        static (SyntaxToken, int) LexLessOrAssignmentChar(int pos , int line, char next)
        {
            if(next == '=')
                return (new SyntaxToken(SyntaxKind.LessOrEqualToken, line, pos, "<=", null!), pos +2);
            
            else if(next == '-')
                return (new SyntaxToken(SyntaxKind.AssignmentToken, line, pos, "<-", null!), pos +2);
            
            return(new SyntaxToken(SyntaxKind.LessToken, line, pos, "<", null!), pos +1);
        }

        static (SyntaxToken, int) LexEqualsChar(int pos, int line, char next)
        {
            if(next == '=')
                return (new SyntaxToken(SyntaxKind.EqualToken, line, pos , "==", null!), pos+2);

            return WrongNext(pos,line,next);
        }
        static(SyntaxToken, int) LexAndsChar(int pos, int line, char next)
        {
            if(next == '&')
                return (new SyntaxToken(SyntaxKind.AndAndToken,line, pos, "&&", null!), pos +2);

            return WrongNext(pos,line,next);
        }
        
        static (SyntaxToken, int) LexOrsChar(int pos, int line, char next)
        {
            if(next == '|')
                return (new SyntaxToken(SyntaxKind.OrOrToken, line, pos, "||", null!), pos +2);
            
            return WrongNext(pos,line,next);
        }

        static (SyntaxToken, int) WrongNext(int pos, int line, char next)
        {
            Error.SetError("Lexical",$"Unexpected character at line {line}, position {pos}");

            return (new SyntaxToken(SyntaxKind.ErrorToken,line,pos,next.ToString(),null!),pos+1);
        }
    }
}