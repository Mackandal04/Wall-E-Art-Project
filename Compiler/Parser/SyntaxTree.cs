using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

    public abstract class InstructionNode : ASTNode
    {
        protected InstructionNode(int line) : base(line) { }
    }

    public abstract class ExpressionNode : ASTNode
    {
        protected ExpressionNode(int line) : base(line) { }
    }

    public class ProgramNode : ASTNode
    {
        public List<InstructionNode> Instructions { get; }

        public ProgramNode(IEnumerable<InstructionNode> instructions, int line) : base(line)
        {
            Instructions = instructions.ToList();
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
    }

    public class LiteralNode : ExpressionNode
    {
        //numb,string, bool

        public object Value { get; }

        public LiteralNode(object value, int line) : base(line)
        {
            Value = value;
        }
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
    }

    #endregion

    #region INSTRUCTIONS
    public class SpawnNode : InstructionNode
    {
        public ExpressionNode XExpression { get; }
        public ExpressionNode YExpression { get; }

        public SpawnNode(ExpressionNode XExpression, ExpressionNode YExpression, int line) : base(line)
        {
            XExpression = XExpression;
            YExpression = YExpression;
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
    }

    public class SizeNode : InstructionNode
    {
        public ExpressionNode SizeExpression { get; }
        public SizeNode(ExpressionNode sizeExpression, int line) : base(line)
        {
            SizeExpression = sizeExpression;
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
    }

    public class FillNode : InstructionNode
    {
        public FillNode(int line) : base(line) { }
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
    }

    public class LabelNode : InstructionNode
    {
        public string Label { get; }

        public LabelNode(string label, int line) : base(line)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Label invalido");

            Label = label;
        }
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
    }

    public enum BinaryOperator
        {
            Plus,
            Minus,
            Mult,
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