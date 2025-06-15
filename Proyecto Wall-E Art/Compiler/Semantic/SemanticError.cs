using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public class SemanticError
    {
        public string Message { get; }
        public int Line { get; }

        public SemanticError(string message, int line)
        {
            Message = message;

            Line = line;
        }

        public override string ToString() => $"[linea : {Line} {Message}]";
    }
}