using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

var lexerSupplies = new LexingSupplies();

var (token, newPos) = lexerSupplies.LexMathCharacters['+'](0, 1, ' ');

Console.WriteLine(token.Kind);

var kind = lexerSupplies.GetKeywordKind("Red");

Console.WriteLine(kind);

// Reconocer "->" como AssignmentToken
var (Token, NewPos) = lexerSupplies.LexMathCharacters['-'](0, 1, '>');
Console.WriteLine(Token.Kind);