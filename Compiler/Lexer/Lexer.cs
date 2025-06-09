using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    internal sealed class Lexer
    {
        readonly string
        Text;
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

            while (char.IsWhiteSpace(Current) && Current != '\n')
                Advance();

            if (Current == '\n')
                return LexNewLine();

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

            if (char.IsDigit(Current))
                    return LexNumber();

            if (char.IsLetter(Current))
                return LexIdentifierOrLabel();

            string text = Text.Substring(position, 1);
            var errorToken = new SyntaxToken(SyntaxKind.ErrorToken, line, position, text, null!);
            Advance();
            return errorToken;
        }

        SyntaxToken LexNewLine()
        {
            var start = position;
            Advance();
            line++;
            return new SyntaxToken(SyntaxKind.NewLineToken, line, start, "\n", null!);
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
        

        SyntaxToken LexString()
        {
            int start = position;

            Advance(); // Consume la comilla inicial

            bool escaped = false;

            while (Current != '\0' && (Current != '"' || escaped))
            {
                if (Current == '\n')
                {
                    ErrorsCollecter.Add("LEXICAL", "String no puede tener saltos de línea", line);
                    return new SyntaxToken(SyntaxKind.ErrorToken, line, start, Text.Substring(start, position - start), null!);
                }

                // Si vemos una barra invertida y no estamos escapados, marcamos 'escaped = true'
                if (Current == '\\' && !escaped)
                    escaped = true;

                else
                    escaped = false;// Cualquier otro carácter quita el estado escapado

                Advance();
            }

            if (Current == '\0')
            {
                ErrorsCollecter.Add("LEXICAL", "Comillas faltantes en el string", line);
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, Text.Substring(start, position - start), null!);
            }

            Advance(); // Consumimos la comilla final

            string raw = Text.Substring(start, position - start);

            // Ahora extraemos el valor interior (sin comillas) y procesamos escapados
            string value = raw.Substring(1, raw.Length - 2);
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

        SyntaxToken ReportError(string op, int start)
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
                ErrorsCollecter.Add("LEXICAL", $"ID no debe comenzar con numero o guion{Current}", line);
                Advance();
                return new SyntaxToken(SyntaxKind.ErrorToken, line, start, Text.Substring(start, 1), null!);
            }

            Advance();

            while (char.IsLetterOrDigit(Current) || Current == '_' || Current == '-')
                Advance();

            int identLenght = position - start;
            string identText = Text.Substring(start, identLenght);

            // if (Current == '\n')
            // {
            //     return new SyntaxToken(SyntaxKind.LabelToken, line, start, identText, identText);
            // }

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
                    ErrorsCollecter.Add("LEXICAL", $"Token no valido : '{token.Text}'", token.Line);

                yield return token;

            }
            while (token.Kind != SyntaxKind.EndOfFileToken);
        }
        
        #endregion
    }
}