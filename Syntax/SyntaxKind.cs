namespace Proyecto_Wall_E_Art.Syntax
{
        public enum SyntaxKind
    {
        // Basics Tokens
        NumberToken,
        StringToken,
        IdentifierToken,
        EndOfLineToken,

        // Operators
        PlusToken,
        MinusToken,
        MultToken,
        DivisionToken,
        ModToken,
        PowToken,
        AssignmentToken,

        // Compare
        GreaterToken,
        LessToken,
        EqualToken,
        NotEqualToken,
        GreaterOrEquealToken,
        LessOrEqualToken,
        AndAndToken,
        OrOrToken,

        // Symbols
        OpenParenthesisToken,
        CloseParenthesisToken,
        ComaToken,

        // Commands
        SpawnToken,
        ColorToken,
        SizeToken,
        DrawLineToken,
        DrawCircleToken,
        DrawRectangleToken,
        FillToken,
        GoToToken,

        // Functions
        GetActualXToken,
        GetActualYToken,
        GetCanvasSizeToken,
        GetColorCountToken,
        IsBrushColorToken,
        IsBrushSizeToken,
        IsCanvasColorToken,

        // Colors
        RedKeyword,
        BlueKeyword,
        GreenKeyword,
        YellowKeyword,
        OrangeKeyword,
        PurpleKeyword,
        BlackKeyword,
        WhiteKeyword,
        TransparentKeyword,

        // Etiquetas
        LabelToken,

        // Control
        EndOfFileToken
    }
}