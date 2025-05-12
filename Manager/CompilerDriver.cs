using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public class CompilerDriver
    {
        public bool ContainsError { get; }
        public readonly IEnumerable<SyntaxToken> tokens;

        public CompilerDriver(string text)
        {
            var lexer = new Lexer(text);
            
            this.tokens = lexer.LexAll();
            
        }
    }
}