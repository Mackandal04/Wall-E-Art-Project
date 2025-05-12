using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    internal sealed class Lexer 
    { 
        private readonly string Text; 
        private int position = 0; 
        private int line = 1;
        private char Current => Peek(0);
        private char NextChar => Peek(1);

        public Lexer(string text)
        {
            Text = text ?? string.Empty;
        }

        public SyntaxToken Lex() //Recorrer caracter a caracter
        {
            if (position >= Text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, line, position, string.Empty, null!);

            if (Current == '"')
                return LexString();

            if (Current == '<' && NextChar == '-')
                return LexAssignment();

            if ((Current == '=' && NextChar == '=') ||
                (Current == '<' && NextChar == '=') ||
                (Current == '>' && NextChar == '=') ||
                (Current == '&' && NextChar == '&') ||
                (Current == '|' && NextChar == '|'))
                return LexTwoCharOperator();

            if (SingleCharTokens.TryGetValue(Current, out var singleKind))
            {
                var tok = new SyntaxToken(singleKind, line, position, Current.ToString(), null!);
                position++;
                return tok;
            }

            if (LexingSupplies.LexMathCharacters.TryGetValue(Current,out Func<int, int, char, (SyntaxToken token, int newPos)> opFunc))
            {
                var (token, newPos) = opFunc(position, line, NextChar);
                position = newPos;
                return token;
            }

            if (char.IsWhiteSpace(Current))
                return LexWhitespace();

            if (char.IsDigit(Current))
                return LexNumber();

            if (char.IsLetter(Current))
                return LexIdentifier();

            // Unexpected character
            Error.SetError("LEXICAL", $"Line {line}: Unexpected character '{Current}'");
            var errorToken = new SyntaxToken(SyntaxKind.ErrorToken, line, position, Current.ToString(), null!);
            position++;
            return errorToken;
        }

        public IEnumerable<SyntaxToken> LexAll()
        {
            SyntaxToken token;
            
            do
            {
                token = Lex();
                
                if (token.Kind == SyntaxKind.ErrorToken)
                {
                    System.Console.WriteLine($"WrongToken -{token.Text}");
                    throw new Exception("InvalidToken");
                }

                yield return token;

            } 
            while (token.Kind != SyntaxKind.EndOfFileToken);
        }

        #region Helpers

        private char Peek(int offset)
        {
            var idx = position + offset;
            return idx >= Text.Length ? '\0' : Text[idx];
        }

        private void Advance(int count = 1)
        {
            position += count;
        }

        private static readonly Dictionary<char, SyntaxKind> SingleCharTokens = new()
        {
            ['('] = SyntaxKind.OpenParenthesisToken,
            [')'] = SyntaxKind.CloseParenthesisToken,
            ['['] = SyntaxKind.OpenBracketToken,
            [']'] = SyntaxKind.CloseBracketToken,
            [','] = SyntaxKind.CommaToken,
        };

        #endregion

        private SyntaxToken LexString() //Lexeando cada token
        {
            var start = position;
            Advance();
            bool escaped = false;

            while (Current != '\0' && (Current != '"' || escaped))
            {
                if (Current == '\\')
                    escaped = !escaped;
                else
                    escaped = false;

                if (Current == '\n')
                    line++;

                Advance();
            }
            if (Current == '\0')
            {
                Error.SetError("SYNTAX", $"Line {line}: Unterminated string literal");
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, Text.Substring(start, position - start), null!);
            }

            Advance();
            var raw = Text.Substring(start, position - start);
            var value = ParsingSupplies.BackSlashEval(raw, line)[1..^1];

            return Error.wrong
                ? new SyntaxToken(SyntaxKind.ErrorToken, line, start, raw, null!)
                : new SyntaxToken(SyntaxKind.StringToken, line, start, raw, value);
        }

        private SyntaxToken LexAssignment()
        {
            var start = position;
            Advance(2);
            return new SyntaxToken(SyntaxKind.AssignmentToken, line, start, "<-", null!);
        }

        private SyntaxToken LexTwoCharOperator()
        {
            int start = position;
            string op = Text.Substring(position, 2);

            Advance(2);

            if(op == "==")
                return (new SyntaxToken(SyntaxKind.EqualToken,line,start,op, null!));

            if(op == ">=")
                return (new SyntaxToken(SyntaxKind.GreaterOrEqualToken,line,start,op, null!));

            if(op == "<=")
                return (new SyntaxToken(SyntaxKind.LessOrEqualToken,line,start,op, null!));

            if(op == "&&")
                return (new SyntaxToken(SyntaxKind.AndAndToken,line,start,op, null!));

            if(op == "||")
                return (new SyntaxToken(SyntaxKind.OrOrToken,line,start,op, null!));

            
            Error.SetError("Lexical",$"Unexpected character{op} at line {line}");

            return (new SyntaxToken(SyntaxKind.ErrorToken,line,start,op, null!));
        }

        private SyntaxToken LexWhitespace()
        {
            var start = position;
            while (char.IsWhiteSpace(Current))
            {
                if (Current == '\n')
                    line++;
                Advance();
            }
            var text = Text.Substring(start, position - start);
            return new SyntaxToken(SyntaxKind.WhitespaceToken, line, start, text, null!);
        }

        private SyntaxToken LexNumber()
        {
            var start = position;
            while (char.IsDigit(Current))
                Advance();

            var text = Text.Substring(start, position - start);
            if (!int.TryParse(text, out var value))
            {
                Error.SetError("LEXICAL", $"Line {line}: Invalid number '{text}'");
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, text, null!);
            }

            return new SyntaxToken(SyntaxKind.NumberToken, line, start, text, value);
        }

        private SyntaxToken LexIdentifier()
        {
            var start = position;
            Advance();

            while (char.IsLetterOrDigit(Current) || Current == '_'  ||Current == '-')
                Advance();

            var text = Text.Substring(start, position - start);
            var kind = LexingSupplies.GetKeywordKind(text);
            var value = kind == SyntaxKind.ColorKeyword ? text : null!;

            return new SyntaxToken(kind, line, start, text, value);
        }
}

}