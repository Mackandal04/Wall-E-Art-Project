using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public sealed class Parser
    {
        List<SyntaxToken> tokens;
        int position;
        SyntaxToken Current => LookAhead(0);//acutal token

        public Parser(IEnumerable<SyntaxToken> tokens)
        {
            this.tokens = tokens.ToList();
            position = 0;
            ParserError.Clear();
        }

        SyntaxToken LookAhead(int offset)
        {
            int index = position + offset;

            if (index < tokens.Count)
                return tokens[index];

            return tokens.Last();
        }

        SyntaxToken NextToken()//Se traga el token y sigue para el sgte
        {
            var token = Current;
            position++;
            return token;
        }

        SyntaxToken Match(SyntaxKind kind, string errorMsg)
        {
            //Valida el token y sigue, de lo contrario lanza error y para
            if (Current.Kind == kind)
                return NextToken();

            //Recordar el ErrorParserSetError
            Error.SetError("SYNTAX", errorMsg + $"(found{Current.Text})");
            return new SyntaxToken(SyntaxKind.ErrorToken, Current.Line, Current.Position, Current.Text, null!);
        }

        public List<InstructionNode> ParseProgram()
        {
            var instructions = new List<InstructionNode>();

            while (Current.Kind != SyntaxKind.EndOfFileToken)
            {
                instructions.Add(ParseInstruction());
            }

            return instructions;
        }

        ExpressionNode ParseExpression(int parentPrecedence = 0)
        {
            ExpressionNode left;

            if (ParsingSupplies.GetUnaryOperatorPrecedence(Current.Kind) > 0)
            {
                var op = NextToken();
                var operand = ParseExpression(ParsingSupplies.GetUnaryOperatorPrecedence(op.Kind));
                left = new UnaryExpressionNode(op, operand);
            }
            else
                left = ParsePrimary();

            while (true)
            {
                var precedence = ParsingSupplies.GetBinaryOperatorPrecedence(Current.Kind);
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var op = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionNode(left, op, right);
            }

            return left;
        }

        private ExpressionNode ParsePrimary()
        {
            if (Current.Kind == SyntaxKind.NumberToken)
                return new LiteralNode(NextToken());

            if (Current.Kind == SyntaxKind.IdentifierToken)
            {
                if (LookAhead(1).Kind == SyntaxKind.OpenParenthesisToken)
                    return ParseFunctionCall();

                return new VariableNode(NextToken());
            }

            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                NextToken();

                var expr = ParseExpression();

                Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')'");

                return expr;
            }

            Error.SetError("SYNTAX", $"Se esperaba expresión primaria, se encontro '{Current.Text}'");
            return new LiteralNode(new SyntaxToken(SyntaxKind.ErrorToken, Current.Line, Current.Position, Current.Text, null!));
        }

        InstructionNode ParseInstruction()//Escoge el parseo segun el token
        {
            if (Current.Kind == SyntaxKind.IdentifierToken)
            {
                if (LookAhead(1).Kind == SyntaxKind.AssignmentToken)
                    return ParseAssignment();
            }

            switch (Current.Kind)
            {
                case SyntaxKind.SpawnKeyword: return ParseSpawn();
                case SyntaxKind.ColorKeyword: return ParseColor();
                case SyntaxKind.SizeKeyword: return ParseSize();
                // case SyntaxKind.DrawLineKeyword: return ParseDrawLine();
                // case SyntaxKind.DrawCircleKeyword: return ParseDrawCircle();
                // case SyntaxKind.DrawRectangleKeyword: return ParseDrawRectangle();
                // case SyntaxKind.FillKeyword: return ParseFill();
                // case SyntaxKind.GoToKeyword: return ParseGoTo();
                // case SyntaxKind.GetActualXKeyword: return ParseGetActualX();
                // case SyntaxKind.GetActualYKeyword: return ParseGetActualY();
                // case SyntaxKind.GetCanvasSizeKeyword: return ParseGetCanvasSize();
                // case SyntaxKind.GetColorCountKeyword: return ParseGetColorCount();
                // case SyntaxKind.IsBrushColorKeyword: return ParseIsBrushColor();
                // case SyntaxKind.IsBrushSizeKeyword: return ParseIsBrushSize();
                // case SyntaxKind.IsCanvasColorKeyword: return ParseIsCanvasColor();

                default:
                    Error.SetError("SYNTAX", $"Unexpected token '{Current.Text}'");
                    NextToken();
                    return null!;
            }
        }

        //Falta analizar los label
        InstructionNode ParseAssignment()
        {
            var idToken = NextToken(); //Consume la variable

            Match(SyntaxKind.AssignmentToken, "Se esperaba <- para asignacion");

            var expression = ParseExpression();

            return new AssignmentNode(idToken, expression);
        }

        InstructionNode ParseSpawn()
        {
            NextToken();

            Match(SyntaxKind.OpenParenthesisToken, "Se esperaba un '(' despues de 'Spawn' ");

            var leftExpression = ParseExpression();

            Match(SyntaxKind.CommaToken, "Se esperana una ',' despues de la expresion de la izquierda");

            var rightExpression = ParseExpression();

            Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')' despues de la expresion de la derecha");

            return new SpawnNode(leftExpression, rightExpression);
        }
        InstructionNode ParseColor()
        {
            NextToken();

            Match(SyntaxKind.OpenParenthesisToken, "Se esperaba '(' tras Color");

            var color = Match(SyntaxKind.StringToken, "Se esperaba un string de color");

            Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')' despues del color");

            return new ColorNode(color);
        }

        InstructionNode ParseSize()
        {
            NextToken();

            Match(SyntaxKind.OpenParenthesisToken, "Se esperaba '(' tras Size");

            var value = ParseExpression();

            Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')' tras el valor ");

            return new SizeNode(value);
        }
        // InstructionNode ParseGoTo()
        // {
        //     NextToken();

        //     var label = Match(SyntaxKind.IdentifierToken, "Se esperana una etiqueta");

        //     Match(SyntaxKind.OpenParenthesisToken, "Se esperba '(' tras la etiqueta");

        //     var condition = ParseExpression();

        //     Match(SyntaxKind.CloseParenthesisToken, "Se esperba ')' tras la condicion");

        //     return new GoToNode(label, condition);
        // }

        // InstructionNode ParseFill()
        // {
        //     NextToken();

        //     Match(SyntaxKind.OpenParenthesisToken, "Se esperaba un '(' tras Fill");

        //     Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')' ");

        //     return new FillNode();
        // }
    }
}