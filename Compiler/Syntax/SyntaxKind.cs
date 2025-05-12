namespace Proyecto_Wall_E_Art
{
        public enum SyntaxKind
    {
        // Basics
        NumberToken,
        StringToken,
        IdentifierToken,

        // Math Operators
        PlusToken,
        MinusToken,
        MultToken,        // *
        SlashToken,       // /
        ModToken,         // %
        PowToken,       // **

        // Assignment
        AssignmentToken,  // <-



        // Compare
        GreaterToken,
        LessToken,
        EqualToken,
        GreaterOrEqualToken,
        LessOrEqualToken,
        AndAndToken,            // &&
        OrOrToken,              // ||

        // Bool Lierals
        TrueKeyword,    // true
        FalseKeyword,   // false

        // Symbols
        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBracketToken,
        CloseBracketToken,
        CommaToken,

        // Comands
        SpawnKeyword,
        ColorKeyword,
        SizeKeyword,
        DrawLineKeyword,
        DrawCircleKeyword,
        DrawRectangleKeyword,
        FillKeyword,
        GoToKeyword,

        
        // Functions
        GetActualXKeyword,
        GetActualYKeyword,
        GetCanvasSizeKeyword,
        GetColorCountKeyword,
        IsBrushColorKeyword,
        IsBrushSizeKeyword,
        IsCanvasColorKeyword,

        // Others
        LabelToken,       
        WhitespaceToken,
        EndOfFileToken,
        ErrorToken
    }
}