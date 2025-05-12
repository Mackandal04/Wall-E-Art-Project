using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art.Compiler.Parser
{
    public sealed class Parser
    {
        int tokenPosition;

        public List<SyntaxToken> tokens;

        public Parser(IEnumerable<SyntaxToken> tokens)
        {
            
        }
    }
}