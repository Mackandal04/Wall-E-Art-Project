using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art.Compiler.Semantic;

namespace Proyecto_Wall_E_Art
{
    public abstract class ASTNode
    {
        public int Line { get; }

        public ASTNode(int line)
        {
            Line = line;
        }
    }

    public abstract class InstructionNode : ASTNode, ISemanticNode
    {
        protected InstructionNode(int line) : base(line) { }

        public abstract void Validate(SemanticContext context);
    }

    public abstract class ExpressionNode : ASTNode, ISemanticNode
    {
        protected ExpressionNode(int line) : base(line) { }

        //devuelve el tipo resultante y reporta errores
        public abstract string CheckType(SemanticContext semanticContext);

        public void Validate(SemanticContext context)
        {
            CheckType(context);
        }
    }

    public class ProgramNode : ASTNode, ISemanticNode
    {
        public List<InstructionNode> Instructions { get; }

        public ProgramNode(IEnumerable<InstructionNode> instructions, int line) : base(line)
        {
            Instructions = instructions.ToList();
        }

        public void Validate(SemanticContext context)
        {
            foreach (var item in Instructions)
            {
                if (item is AssignmentNode assignmentNode)
                    context.VariablesTable.TryAdd(assignmentNode.Identifier, "desconocido");
            }

            foreach (var item in Instructions)
            {
                item.Validate(context);
            }
        }
    }

    #region EXPRESSIONS
    public class BinaryExpressionNode : ExpressionNode
    {
        // x+y
        public ExpressionNode LeftExpressionNode { get; }
        public BinaryOperator Operator { get; }
        public ExpressionNode RightExpressionNode { get; }

        public BinaryExpressionNode(ExpressionNode leftExpressionNode, BinaryOperator op, ExpressionNode rightExpressionNode, int line) : base(line)
        {
            LeftExpressionNode = leftExpressionNode;
            Operator = op;
            RightExpressionNode = rightExpressionNode;
        }

        public override string CheckType(SemanticContext semanticContext)
        {
            string leftType = LeftExpressionNode.CheckType(semanticContext);
            string rightType = RightExpressionNode.CheckType(semanticContext);

            bool Both(string type) => leftType == type && rightType == type;

            switch (Operator)
            {
                case BinaryOperator.Plus:
                case BinaryOperator.Minus:
                case BinaryOperator.Mult:
                case BinaryOperator.Slash:
                case BinaryOperator.Mod:
                case BinaryOperator.Pow:
                    if (!Both("int"))
                        return Error($"Operador '{Operator}' requiere enteros", semanticContext);
                    return "int";

                case BinaryOperator.Equal:
                    if (leftType != rightType)
                        return Error($"'==' requiere tipos iguales, pero recibió {leftType} y {rightType}", semanticContext);
                    return "bool";

                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                    if (!Both("int"))
                        return Error($"Operador '{Operator}' requiere enteros", semanticContext);
                    return "bool";

                case BinaryOperator.AndAnd:
                case BinaryOperator.OrOr:
                    if (!Both("bool"))
                        return Error($"Operador lógico '{Operator}' requiere booleanos", semanticContext);
                    return "bool";

                default:
                    return Error($"Operador binario no soportado: {Operator}", semanticContext);
            }
        }

        private string Error(string message, SemanticContext semanticContext)
        {
            semanticContext.GetErrors(message, Line);
            return "<desconocido>";
        }
    
    }

    public class UnaryExpressionNode : ExpressionNode
    {
        // -x
        public UnaryOperator Operator { get; }
        public ExpressionNode MiddleExpression { get; }

        public UnaryExpressionNode(UnaryOperator op, ExpressionNode middleExpression, int line) : base(line)
        {
            Operator = op;
            MiddleExpression = middleExpression;
        }

        public override string CheckType(SemanticContext semanticContext)
        {
            string type = MiddleExpression.CheckType(semanticContext);

            return Operator switch
            {
                UnaryOperator.Minus => type == "int" ? "int" : Error("Operador '-' requiere tipo int ", semanticContext),
                UnaryOperator.Not => type == "bool" ? "bool" : Error("Operador '!' requiere tipo bool ", semanticContext),
                _ => Error("Operador unario desconocido", semanticContext)
            };
        }

        string Error(string message, SemanticContext semanticContext)
        {
            semanticContext.GetErrors(message, Line);
            return "desconocido";
        }
    }

    public class LiteralNode : ExpressionNode
    {
        //numb,string, bool

        public object Value { get; }

        public LiteralNode(object value, int line) : base(line)
        {
            Value = value;
        }

        public override string CheckType(SemanticContext semanticContext)
        => Value switch
        {
            int => "int",
            bool => "bool",
            string => "string",
            _ => "desconocido"
        };
    }

    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(string name, int line) : base(line)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Variable name cannot be null or whitespace.", nameof(name));
            Name = name;
        }

        public override string CheckType(SemanticContext semanticContext)
        {
            if (!semanticContext.VariablesTable.TryGetValue(Name, out var type))
            {
                semanticContext.GetErrors($"Variable {Name} no declarada", Line);
                return "unknown";
            }

            return type;
        }
    }

    public class BuiltInFunctionNode : ExpressionNode
    {
        public FunctionKind FunctionKind { get; }
        public List<ExpressionNode> Arguments { get; }

        public BuiltInFunctionNode(FunctionKind functionKind, IEnumerable<ExpressionNode> param, int line) : base(line)
        {
            FunctionKind = functionKind;
            Arguments = param.ToList();
        }

        public override string CheckType(SemanticContext semanticContext)
        {
            // Ejemplo para GetActualX() que no recibe nada y devuelve int
            if (FunctionKind == FunctionKind.GetActualX)
            {
                if (Arguments.Count != 0)
                    semanticContext.GetErrors($"'{FunctionKind}' no recibe argumentos", Line);
                return "int";
            }

            // Otras funciones...
            return "<desconocido>";
        }
    }

    public class InvalidExpressionNode : ExpressionNode
    {
        public string Why { get; }
        public InvalidExpressionNode(string why, int line) : base(line)
        {
            Why = why;
        }

        public override string CheckType(SemanticContext semanticContext)
        {
            semanticContext.GetErrors($"Expresión inválida: {Why}", Line);
            return "desconocido";
        }
    }

    #endregion

    #region INSTRUCTIONS
    public class SpawnNode : InstructionNode
    {
        public ExpressionNode XExpression { get; }
        public ExpressionNode YExpression { get; }

        public SpawnNode(ExpressionNode xExpression, ExpressionNode yExpression, int line) : base(line)
        {
            XExpression = xExpression;
            YExpression = yExpression;
        }

        public override void Validate(SemanticContext context)
        {
            SemanticHelp.CheckParams(context, "Spawn", "int",
                ("X", XExpression),
                ("Y", YExpression));
        }
    }

    public class ColorNode : InstructionNode
    {
        public string Color { get; }

        public ColorNode(string color, int line) : base(line)
        {
            if (string.IsNullOrEmpty(color))
                throw new ArgumentException("Color invalido");

            Color = color;
        }

        public override void Validate(SemanticContext context)
        {
            //y puede venir de una varibale?
            if (!context.ColorsTable.Contains(Color))
                context.GetErrors($"Color '{Color}' no declarado", Line);
        }
    }

    public class SizeNode : InstructionNode
    {
        public ExpressionNode SizeExpression { get; }
        public SizeNode(ExpressionNode sizeExpression, int line) : base(line)
        {
            SizeExpression = sizeExpression;
        }

        public override void Validate(SemanticContext context)
        {
            SemanticHelp.CheckParams(context, "Size", "int",
                ("size", SizeExpression));
        }
    }

    public class DrawLineNode : InstructionNode
    {
        public ExpressionNode DirXExpression { get; }
        public ExpressionNode DirYExpression { get; }
        public ExpressionNode DistanceExpression { get; }

        public DrawLineNode(ExpressionNode dirXExpression, ExpressionNode dirYExpression, ExpressionNode distanceExpression, int line) : base(line)
        {
            DirXExpression = dirXExpression;
            DirYExpression = dirYExpression;
            DistanceExpression = distanceExpression;
        }

        public override void Validate(SemanticContext context)
        {
            SemanticHelp.CheckParams(context, "DrawLine", "int",
                ("Coordenada X", DirXExpression),
                ("Coordenada Y", DirYExpression),
                ("Distancia", DistanceExpression));
        }
    }

    public class DrawCircleNode : InstructionNode
    {
        public ExpressionNode DirXExpression { get; }
        public ExpressionNode DirYExpression { get; }
        public ExpressionNode RadiusExpression { get; }

        public DrawCircleNode(ExpressionNode dirXExpression, ExpressionNode dirYExpression, ExpressionNode radiusExpression, int line) : base(line)
        {
            DirXExpression = dirXExpression;
            DirYExpression = dirYExpression;
            RadiusExpression = radiusExpression;
        }

        public override void Validate(SemanticContext context)
        {
            SemanticHelp.CheckParams(context, "DrawCircle", "int",
                ("Coordenada X", DirXExpression),
                ("Coordenada Y", DirYExpression),
                ("Radio", RadiusExpression));
        }
    }

    public class DrawRectangleNode : InstructionNode
    {
        public ExpressionNode DirXExpression { get; }
        public ExpressionNode DirYExpression { get; }
        public ExpressionNode DistanceExpression { get; }
        public ExpressionNode WidthExpression { get; }
        public ExpressionNode HeightExpression { get; }

        public DrawRectangleNode(ExpressionNode dirXExpression, ExpressionNode dirYExpression, ExpressionNode distanceExpression, ExpressionNode widthExpression, ExpressionNode heightExpression, int line) : base(line)
        {
            DirXExpression = dirXExpression;
            DirYExpression = dirYExpression;
            DistanceExpression = distanceExpression;
            WidthExpression = widthExpression;
            HeightExpression = heightExpression;
        }

        public override void Validate(SemanticContext context)
        {
            SemanticHelp.CheckParams(context, "DrawRectangle", "int",
                ("Coordenada X", DirXExpression),
                ("Coordenada Y", DirYExpression),
                ("Distancia", DistanceExpression),
                ("Ancho", WidthExpression),
                ("Alto", HeightExpression));
        }
    }

    public class FillNode : InstructionNode
    {
        public FillNode(int line) : base(line) { }

        public override void Validate(SemanticContext context){}
    }

    #endregion

    #region EXTRAS


    public class AssignmentNode : InstructionNode
    {
        public string Identifier { get; }
        public ExpressionNode Expression { get; }

        public AssignmentNode(string identifier, ExpressionNode expression, int line) : base((line))
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("Variable invalida");

            Identifier = identifier;
            Expression = expression;
        }

        public override void Validate(SemanticContext context)
        {
            string expressionType = Expression.CheckType(context);

            if (context.VariablesTable.TryGetValue(Identifier, out var variableType) && variableType != "desconocido" && variableType != expressionType)
                context.GetErrors($"Variable {Identifier} ya es de tipo {variableType}, no se puede asignar '{expressionType}' ", Line);

            else
                context.VariablesTable[Identifier] = expressionType;
        }
    }

    public class LabelNode : InstructionNode
    {
        public string LabelName { get; }

        public LabelNode(string label, int line) : base(line)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Label invalido");

            LabelName = label;
        }

        public override void Validate(SemanticContext context){}
    }

    public class GoToNode : InstructionNode
    {
        public string Label { get; }
        public ExpressionNode Condition { get; }

        public GoToNode(string label, ExpressionNode condition, int line) : base(line)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Label invalido");

            if (condition == null)
                throw new ArgumentNullException(nameof(condition), "Condition cannot be null");

            Label = label;
            Condition = condition;
        }

        public override void Validate(SemanticContext context)
        {
            if (!context.LabelsTable.ContainsKey(Label))
            {
                context.GetErrors($"Label {Label} no declarado", Line);
            }

            string conditionType = Condition.CheckType(context);

            if (conditionType != "bool")
                context.GetErrors($"Condicion de tipo {conditionType}, se esperaba 'bool'", Line);
        }
    }

    public enum BinaryOperator
        {
            Plus,
            Minus,
            Mult,
            Pow,
            Slash,
            Mod,
            LessThan,
            GreaterThan,
            LessThanOrEqual,
            GreaterThanOrEqual,
            Equal,
            NotEqual,
            AndAnd,
            OrOr
        }

        public enum UnaryOperator
        {
            Minus,
            Not
        }

        public enum FunctionKind
        {
            GetActualX,
            GetActualY,
            GetCanvasSize,
            GetColorCount,
            IsBrushColor,
            IsBrushSize,
            IsCanvasColor
        }
        #endregion
}