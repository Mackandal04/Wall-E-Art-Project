using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public class CompilerDriver
    {
        public IEnumerable<SyntaxToken> tokens {get;}
        public List<SyntaxNode>SyntaxTree{get;}
        public CompilerDriver(string text)
        {
            var lexer = new Lexer(text);
            tokens = lexer.LexAll().ToList();
            var parser = new Parser(tokens);
        }
    }
}
