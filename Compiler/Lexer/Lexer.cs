using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    internal sealed class Lexer
    {
        readonly string Text;
        int position = 0;
        int line = 1;
        char Current => Peek(0);
        char NextChar => Peek(1);

        public Lexer(string text)
        {
            //Si recibe null lo convierte en el string vacio
            Text = text ?? string.Empty;
        }

        public SyntaxToken Lex() //Recorrer y devolver token a token
        {
            if (position >= Text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, line, position, string.Empty, null!);

            if (Current == '<' && NextChar == '-')
                return LexAssignment();

            if (Current == '"')
                return LexString();

            if (IsTwoCharOperator())
                return LexTwoCharOperator();

            if (SingleCharTokens.TryGetValue(Current, out var singleKind))
            {
                var tok = new SyntaxToken(singleKind, line, position, Current.ToString(), null!);
                Advance();
                return tok;
            }

            if (char.IsWhiteSpace(Current))
                return LexWhitespace();

            if (char.IsDigit(Current))
                return LexNumber();

            if (char.IsLetter(Current))
                return LexIdentifierOrLabel();

            string text = Text.Substring(position, 1);
            var errorToken = new SyntaxToken(SyntaxKind.ErrorToken, line, position, text, null!);
            Advance();
            return errorToken;
        }

        #region Main

        char Peek(int offset)
        {
            var idx = position + offset;
            return idx >= Text.Length ? '\0' : Text[idx];
        }

        void Advance(int count = 1)
        {
            position += count;
        }

        SyntaxToken LexString() //Lexeando cada token
        {
            var start = position;
            Advance();
            bool escaped = false;

            while (Current != '\0' && (Current != '"' || escaped))
            {
                if (Current == '\n')
                {
                    ErrorsCollecter.Add("LEXICAL", "String no puede tener saltos de linea", line);
                    return new SyntaxToken(SyntaxKind.ErrorToken, line, position, Text.Substring(start), null!);
                }
            }

            if (Current == '\0')
            {
                ErrorsCollecter.Add("lEXICAL", $"Comillas faltantes en el string", line);
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, Text.Substring(start, position - start), null!);
            }

            Advance();
            var raw = Text.Substring(start, position - start); //raw almacena el texto original

            var value = raw.Substring(1, raw.Length - 2); //elimina las comillas del principio y fin

            string processed = value.Replace("\\\"", "\"").Replace("\\\\", "\\");

            return new SyntaxToken(SyntaxKind.StringToken, line, start, raw, processed);
        }

        SyntaxToken LexNumber()
        {
            var start = position;
            while (char.IsDigit(Current))
                Advance();

            var text = Text.Substring(start, position - start);
            if (!int.TryParse(text, out var value))
            {
                ErrorsCollecter.Add("LEXICAL", $"Numero invalido {text}", line);
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, text, null!);
            }

            return new SyntaxToken(SyntaxKind.NumberToken, line, start, text, value);
        }


        SyntaxToken LexWhitespace()
        {
            // tambien se analiza \n
            var start = position;

            if (Current == '\n')
            {
                position++;
                line++;
                return new SyntaxToken(SyntaxKind.NewLineToken, line, start, "\n", null!);
            }

            while (char.IsWhiteSpace(Current) && Current != '\n')
            {
                Advance();
            }

            var text = Text.Substring(start, position - start);

            return new SyntaxToken(SyntaxKind.WhitespaceToken, line, start, text, null!);
        }

        SyntaxToken LexAssignment()
        {
            var start = position;
            Advance(2);  //consume <-
            return new SyntaxToken(SyntaxKind.AssignmentToken, line, start, "<-", null!);
        }

        bool IsTwoCharOperator()
        {
            if ((Current == '=' && NextChar == '=') ||
                    (Current == '<' && NextChar == '=') ||
                    (Current == '>' && NextChar == '=') ||
                    (Current == '&' && NextChar == '&') ||
                    (Current == '|' && NextChar == '|') ||
                    (Current == '*' && NextChar == '*'))
            {
                return true;
            }

            return false;
        }


        SyntaxToken LexTwoCharOperator()
        {
            int start = position;

            string op = Text.Substring(position, 2);

            SyntaxKind kind = op switch
            {
                "==" => SyntaxKind.EqualToken,
                ">=" => SyntaxKind.GreaterOrEqualToken,
                "<=" => SyntaxKind.LessOrEqualToken,
                "&&" => SyntaxKind.AndAndToken,
                "||" => SyntaxKind.OrOrToken,
                "**" => SyntaxKind.PowToken,
                _ => SyntaxKind.ErrorToken
            };

            Advance(2);

            return kind == SyntaxKind.ErrorToken ? ReportError(op, start) : new SyntaxToken(kind, line, start, op, null!);
        }

        private SyntaxToken ReportError(string op, int start)
        {
            ErrorsCollecter.Add("LEXICAL", $"Token invalido{op}", line);
            return new SyntaxToken(SyntaxKind.ErrorToken, line, start, op, null!);
        }

        static readonly Dictionary<char, SyntaxKind> SingleCharTokens = new()
        {
            ['('] = SyntaxKind.OpenParenthesisToken,
            [')'] = SyntaxKind.CloseParenthesisToken,
            ['['] = SyntaxKind.OpenBracketToken,
            [']'] = SyntaxKind.CloseBracketToken,
            [','] = SyntaxKind.CommaToken,
            ['+'] = SyntaxKind.PlusToken,
            ['-'] = SyntaxKind.MinusToken,
            ['*'] = SyntaxKind.MultToken,
            ['/'] = SyntaxKind.SlashToken,
            ['%'] = SyntaxKind.ModToken,
            ['<'] = SyntaxKind.LessToken,
            ['>'] = SyntaxKind.GreaterToken,
        };
        SyntaxToken LexIdentifierOrLabel()
        {
            var start = position;

            if (char.IsDigit(Current) || Current == '-')
            {
                ErrorsCollecter.Add("LEXICAL", $"Identifier invalido{Current}", line);
                Advance();
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, Text.Substring(start, 1), null!);
            }

            Advance();

            while (char.IsLetterOrDigit(Current) || Current == '_' || Current == '-')
                Advance();

            int identLenght = position - start;
            string identText = Text.Substring(start, identLenght);

            if (Current == '\n')
            {
                return new SyntaxToken(SyntaxKind.LabelToken, line, start, identText, identText);
            }

            SyntaxKind kind = LexingSupplies.GetKeywordKind(identText);

            return new SyntaxToken(kind, line, start, identText, null!);
        }
        
        public IEnumerable<SyntaxToken> LexAll()
        {
            SyntaxToken token;

            do
            {
                token = Lex();

                if (token.Kind == SyntaxKind.ErrorToken)
                {
                    throw new Exception($"Token invalido -{token.Text}");
                }

                yield return token;

            }
            while (token.Kind != SyntaxKind.EndOfFileToken);
        }
        
        #endregion
    }
}