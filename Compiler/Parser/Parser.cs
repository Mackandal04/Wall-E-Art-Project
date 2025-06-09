using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public sealed class Parser
    {
        public IReadOnlyDictionary<string, int> labelsOfParse => labelsTable;
        List<SyntaxToken> Tokens;
        Dictionary<string, int> labelsTable = new Dictionary<string, int>();
        int Position;
        SyntaxToken Current => LookAhead(0);//acutal token
        bool IsAtEnd => Current.Kind == SyntaxKind.EndOfFileToken;

        public Parser(IEnumerable<SyntaxToken> tokens)
        {
            Tokens = tokens.ToList();

            Position = 0;
        }

        SyntaxToken LookAhead(int advance)
        {
            int index = Position + advance;

            if (index < Tokens.Count)
                return Tokens[index];

            return Tokens.Last();
        }

        SyntaxToken NextToken()//Se traga el token y sigue para el sgte
        {
            var token = Current;
            Position++;
            return token;
        }

        SyntaxToken Match(SyntaxKind kind, string errorMsg)
        {
            //Valida el token y sigue, de lo contrario lanza error y para
            if (Current.Kind == kind)
                return NextToken();

            ErrorsCollecter.Add("SYNTAX", errorMsg , Current.Line);

            //Para evitar toda una linea innecesaria de errores
            var wrong = Current;

            while (!IsAtEnd && Current.Kind != SyntaxKind.NewLineToken)
                NextToken();

            return wrong;
        }

        public ProgramNode ParseProgram()
        {
            //retorna el AST completo de un programa

            ErrorsCollecter.ErrorsClear();

            var instructions = new List<InstructionNode>();

            while (Current.Kind == SyntaxKind.WhitespaceToken)
                NextToken();
                
            if (Current.Kind != SyntaxKind.SpawnKeyword)
            {
                ErrorsCollecter.Add("SYNTAX", "El programa debe comenzar con Spawn", Current.Line);

                while (!IsAtEnd && Current.Kind != SyntaxKind.NewLineToken)
                    NextToken();
            }

            else
                instructions.Add(ParseSpawn());

            //Parsea hasta el final
            while (!IsAtEnd)
            {
                if (Current.Kind == SyntaxKind.NewLineToken)
                {
                    NextToken();
                    continue;
                }

                var instr = ParseInstruction();

                if (instr != null)
                    instructions.Add(instr);
            }
            return new ProgramNode(instructions, Current.Line);
        }

        InstructionNode ParseInstruction()
        {
            //Escoge el parseo segun el token

            if (Current.Kind == SyntaxKind.IdentifierToken && LookAhead(1).Kind == SyntaxKind.NewLineToken)
            {
                var labelToken = NextToken();

                NextToken();//se traga newLine
                
                labelsTable.Add(labelToken.Text, labelToken.Line);

                return new LabelNode(labelToken.Text, labelToken.Line);
            }

            switch (Current.Kind)
                {
                    case SyntaxKind.SpawnKeyword: return ParseSpawn();
                    case SyntaxKind.ColorKeyword: return ParseColor();
                    case SyntaxKind.SizeKeyword: return ParseSize();
                    case SyntaxKind.DrawLineKeyword: return ParseDrawLine();
                    case SyntaxKind.DrawCircleKeyword: return ParseDrawCircle();
                    case SyntaxKind.DrawRectangleKeyword: return ParseDrawRectangle();
                    case SyntaxKind.FillKeyword: return ParseFill();
                    case SyntaxKind.IdentifierToken when LookAhead(1).Kind == SyntaxKind.AssignmentToken: return ParseAssignment();
                    //case SyntaxKind.LabelToken: return ParseLabel();
                    case SyntaxKind.GoToKeyword: return ParseGoTo();


                    //si no coincide con una instruccion valida registra el error
                    default:
                        ErrorsCollecter.Add("SYNTAX", $"Instruccion desconocida: {Current.Text}", Current.Line);

                        while (!IsAtEnd && Current.Kind != SyntaxKind.NewLineToken)
                            NextToken();

                        return null!;
                }
        }


        List<ExpressionNode> ParseParameters()
        {
            Match(SyntaxKind.OpenParenthesisToken, "Se esperaba un '(' ");

            List<ExpressionNode> args = new List<ExpressionNode>();

            if (Current.Kind != SyntaxKind.CloseParenthesisToken)
            {
                //hay parametros
                args.Add(ParseExpression());

                while (Current.Kind == SyntaxKind.CommaToken)
                {
                    NextToken(); //trrago la coma
                    args.Add(ParseExpression());
                }
            }

            Match(SyntaxKind.CloseParenthesisToken, "Se esperaba un ')' antes de finalizar ");

            return args;
        }

        // private InstructionNode ParseLabel()
        // {
        //     var labelToken = Match(SyntaxKind.LabelToken, "Se esperaba un label");

        //     return new LabelNode(labelToken.Text, labelToken.Line);
        // }
        private InstructionNode ParseGoTo()
        {
            Match(SyntaxKind.GoToKeyword, "Se esperaba 'GoTo'");

            Match(SyntaxKind.OpenBracketToken, "Se esperaba '['");
        
            var labelToken = Match(SyntaxKind.IdentifierToken, "Se esperaba un identifier dentro de GoTo");

            if (!labelsTable.ContainsKey(labelToken.Text))
            {
                ErrorsCollecter.Add("SEMANTIC", $"La etiqueta {labelToken.Text} no existe en el contexto actual", Current.Line);
            }

            Match(SyntaxKind.CloseBracketToken, "Se esperaba ']'");

            Match(SyntaxKind.OpenParenthesisToken, "Se esperaba '(' tras el label");

            var condition = ParseExpression();

            Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')'");

            return new GoToNode(labelToken.Text, condition, labelToken.Line);

        }


        InstructionNode ParseFill()
        {
            Match(SyntaxKind.FillKeyword, "Se esperaba 'Fill' ");

            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var args = ParseParameters();
                if (args.Count > 0)
                    ErrorsCollecter.Add("SYNTAX", "Fill no recibe parámetros", Current.Line);
            }

            else
                ErrorsCollecter.Add("SYNTAX", "Se esperaba () despues de Fill", Current.Line);

            return new FillNode(Current.Line);
        }

        ExpressionNode ParseExpression(int parentPrecedence = 0)
        {
            return ParseBinaryExpression(parentPrecedence);


            // ExpressionNode left;

            // if (ParsingSupplies.GetUnaryOperatorPrecedence(Current.Kind) > 0)
            // {
            //     var op = NextToken();
            //     var operand = ParseExpression(ParsingSupplies.GetUnaryOperatorPrecedence(op.Kind));
            //     left = new UnaryExpressionNode(op, operand);
            // }
            // else
            //     left = ParsePrimary();

            // while (true)
            // {
            //     var precedence = ParsingSupplies.GetBinaryOperatorPrecedence(Current.Kind);
            //     if (precedence == 0 || precedence <= parentPrecedence)
            //         break;

            //     var op = NextToken();
            //     var right = ParseExpression(precedence);
            //     left = new BinaryExpressionNode(left, op, right);
            // }

            // return left;
        }

        ExpressionNode ParseBinaryExpression(int parentPrecedence)
        {
            ExpressionNode left = ParseUnaryOrPrimary();

            while (true)
            {
                int precedence = Current.Kind.GetBinaryOperatorPrecedence();

                if (precedence == 0 || precedence < parentPrecedence)
                    break;

                var opToken = Current;

                NextToken();

                if (!EsOperadorBinarioValido(opToken.Kind))
                {
                    ErrorsCollecter.Add("SYNTAX", $"Operador binario inesperado: '{opToken.Text}'", opToken.Line);
                    return new InvalidExpressionNode($"Operador inesperado: '{opToken.Text}'", opToken.Line);
                }

                var right = ParseBinaryExpression(precedence + 1);

                var op = MapToBinaryOperator(opToken.Kind);
                left = new BinaryExpressionNode(left, op, right, opToken.Line);
            }

            return left;
        }

        bool EsOperadorBinarioValido(SyntaxKind kind)
        {
            return kind == SyntaxKind.PlusToken
                || kind == SyntaxKind.MinusToken
                || kind == SyntaxKind.MultToken
                || kind == SyntaxKind.SlashToken
                || kind == SyntaxKind.ModToken
                || kind == SyntaxKind.LessToken
                || kind == SyntaxKind.LessOrEqualToken
                || kind == SyntaxKind.GreaterToken
                || kind == SyntaxKind.GreaterOrEqualToken
                || kind == SyntaxKind.EqualToken
                || kind == SyntaxKind.AndAndToken
                || kind == SyntaxKind.OrOrToken
                || kind == SyntaxKind.PowToken;
        }

        ExpressionNode ParseUnaryOrPrimary()
        {
            var unaryPrec = Current.Kind.GetUnaryOperatorPrecedence();

            if (unaryPrec > 0)
            {
                //solo manejamos '-' o '!'
                var opKind = (Current.Kind == SyntaxKind.MinusToken)
                ? UnaryOperator.Minus
                : UnaryOperator.Not;

                var opToken = Current;
                NextToken();

                var operand = ParseUnaryOrPrimary();

                if (operand is InvalidExpressionNode)
                    return new InvalidExpressionNode("Error en operador unario", opToken.Line);
            }

            return ParsePrimary();
        }

        private ExpressionNode ParsePrimary()
        {
            //numbers
            if (Current.Kind == SyntaxKind.NumberToken)
            {
                var text = Current.Text;

                var token = Current;

                NextToken();

                if (int.TryParse(text, out int value))
                    return new LiteralNode(value, Current.Line);

                ErrorsCollecter.Add("SYNTAX", $"Número inválido '{text}'", Current.Line);
                return new InvalidExpressionNode($"Número inválido '{text}'", Current.Line);
            }

            // 2) strings
            if (Current.Kind == SyntaxKind.StringToken)
            {
                var raw = Current.Text;

                var token = Current;

                NextToken();

                var str = raw[1..^1];        // quitar las comillas

                return new LiteralNode(str, Current.Line);
            }

            // 3) bools
            if (Current.Kind == SyntaxKind.TrueKeyword || Current.Kind == SyntaxKind.FalseKeyword)
            {
                bool val = Current.Kind == SyntaxKind.TrueKeyword;

                var token = Current;

                NextToken();

                return new LiteralNode(val, Current.Line);
            }

            // 4) Variable o llamada a función
            if (Current.Kind == SyntaxKind.IdentifierToken)
            {
                var name = Current.Text;

                var token = Current;

                NextToken();

                if (Current.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    var args = ParseParameters();  // lista de ExpressionNode
                    
                    // Mapeo a FunctionKind
                    if (Enum.TryParse<FunctionKind>(name, out var kind))
                        return new BuiltInFunctionNode(kind, args, token.Line);

                    ErrorsCollecter.Add("SYNTAX", $"Función desconocida '{name}'", token.Line);
                    return new InvalidExpressionNode($"Función desconocida '{name}'", token.Line);
                }

                return new VariableNode(name, Current.Line);
            }

            // 5) Subexpresión entre paréntesis
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var token = Current;

                NextToken();

                var expr = ParseExpression();

                Match(SyntaxKind.CloseParenthesisToken, "Se esperaba ')'");

                return expr;
            }

            var wrong = Current;
            ErrorsCollecter.Add("SYNTAX", $"Se esperaba expresión primaria, encontró '{wrong.Text}'", wrong.Line);
            NextToken();
            return new InvalidExpressionNode($"Token inesperado '{wrong.Text}'", wrong.Line);
        }

        InstructionNode ParseAssignment()
        {
            var idToken = Match(SyntaxKind.IdentifierToken, "Se esperaba identifier");

            Match(SyntaxKind.AssignmentToken, "Se esperaba '<-'");

            var expr = ParseExpression();

            return new AssignmentNode(idToken.Text, expr, idToken.Line);
        }

        InstructionNode ParseSpawn()
        {
            Match(SyntaxKind.SpawnKeyword, "Se esperaba un 'Spawn' ");

            var parameters = ParseParameters();

            if (parameters.Count != 2)
                ErrorsCollecter.Add("SYNTAX", "Spawn requiere solo 2 parametros", Current.Line);

            //Si spawn tiene coordenadas las asignamos , si no => (0,0)
            var xExpr = parameters.ElementAtOrDefault(0) ?? new LiteralNode(0, Current.Line);

            var yExpr = parameters.ElementAtOrDefault(1) ?? new LiteralNode(0, Current.Line);

            return new SpawnNode(xExpr, yExpr, Current.Line);
        }

        InstructionNode ParseColor()
        {
            Match(SyntaxKind.ColorKeyword, "Se esperaba 'Color' ");

            var parameters = ParseParameters();

            if (parameters.Count != 1)
            {
                ErrorsCollecter.Add("SYNTAX", "Color requiere solo un parametro", Current.Line);
            }

            var literal = parameters.FirstOrDefault() as LiteralNode;

            var colorText = literal?.Value as string ?? "Transparent";

            return new ColorNode(colorText, Current.Line);
        }

        InstructionNode ParseSize()
        {
            Match(SyntaxKind.SizeKeyword, "Se esperaba 'Size' ");

            var parameters = ParseParameters();

            if (parameters.Count != 1)
                ErrorsCollecter.Add("SYNTAX", "Size solo requiere un parametro", Current.Line);

            var sizeArg = parameters.ElementAtOrDefault(0) ?? new LiteralNode(1, Current.Line);

            return new SizeNode(sizeArg, Current.Line);
        }

        InstructionNode ParseDrawLine()
        {
            Match(SyntaxKind.DrawLineKeyword, "Se esperaba 'DrawLine'");

            var parameters = ParseParameters();

            if (parameters.Count != 3)
                ErrorsCollecter.Add("SYNTAX", "DrawLine requiere 3 parámetros", Current.Line);


            var firstArg = parameters.ElementAtOrDefault(0) ?? new LiteralNode(0, Current.Line);
            var secondArg = parameters.ElementAtOrDefault(1) ?? new LiteralNode(0, Current.Line);
            var thirdArg = parameters.ElementAtOrDefault(2) ?? new LiteralNode(0, Current.Line);

            return new DrawLineNode(firstArg, secondArg, thirdArg, Current.Line);            
        }

        InstructionNode ParseDrawCircle()
        {
            Match(SyntaxKind.DrawCircleKeyword, "Se esperaba 'DrawCircle'");

            var parameters = ParseParameters();

            if (parameters.Count != 3)
                ErrorsCollecter.Add("SYNTAX", "DrawCircle requiere 3 parámetros", Current.Line);


            var firstArg = parameters.ElementAtOrDefault(0) ?? new LiteralNode(0, Current.Line);
            var secondArg = parameters.ElementAtOrDefault(1) ?? new LiteralNode(0, Current.Line);
            var thirdArg = parameters.ElementAtOrDefault(2) ?? new LiteralNode(0, Current.Line);

            return new DrawCircleNode(firstArg, secondArg, thirdArg, Current.Line);
        }

        InstructionNode ParseDrawRectangle()
        {
            Match(SyntaxKind.DrawRectangleKeyword, "Se esperaba 'DrawRectangle'");

            var parameters = ParseParameters();

            if (parameters.Count != 5)
                ErrorsCollecter.Add("SYNTAX", "DrawRectangle solo requiere 5 parámetros", Current.Line);

            var x1 = parameters.ElementAtOrDefault(0) ?? new LiteralNode(0, Current.Line);
            var y1 = parameters.ElementAtOrDefault(1) ?? new LiteralNode(0, Current.Line);
            var x2 = parameters.ElementAtOrDefault(2) ?? new LiteralNode(0, Current.Line);
            var y2 = parameters.ElementAtOrDefault(3) ?? new LiteralNode(0, Current.Line);
            var w  = parameters.ElementAtOrDefault(4) ?? new LiteralNode(0, Current.Line);

            return new DrawRectangleNode(x1, y1, x2, y2, w, Current.Line);
        }

        BinaryOperator MapToBinaryOperator(SyntaxKind syntaxKind)
        {
            return syntaxKind switch
            {
                SyntaxKind.PlusToken => BinaryOperator.Plus,
                SyntaxKind.MinusToken => BinaryOperator.Minus,
                SyntaxKind.MultToken => BinaryOperator.Mult,
                SyntaxKind.SlashToken => BinaryOperator.Slash,
                SyntaxKind.ModToken => BinaryOperator.Mod,
                SyntaxKind.LessToken => BinaryOperator.LessThan,
                SyntaxKind.LessOrEqualToken => BinaryOperator.LessThanOrEqual,
                SyntaxKind.GreaterToken => BinaryOperator.GreaterThan,
                SyntaxKind.GreaterOrEqualToken => BinaryOperator.GreaterThanOrEqual,
                SyntaxKind.EqualToken => BinaryOperator.Equal,
                SyntaxKind.AndAndToken => BinaryOperator.AndAnd,
                SyntaxKind.OrOrToken => BinaryOperator.OrOr,
                SyntaxKind.PowToken => BinaryOperator.Pow,
                _ => throw new InvalidOperationException($"Operador binario inesperado: {syntaxKind}")
            };
        }
    }
}