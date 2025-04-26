using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public sealed class SyntaxToken
    {
        public SyntaxToken Kind{get;}
        public int Position{get;}
        public int Line{get;}
        public string Text{get;}
        public object Value{get;}

        public SyntaxToken(SyntaxToken kind, int position, int line, string text, object value)
        {
            Kind = kind;
            Position = position;
            Line = line;
            Text = text;
            Value = value;
        }
    }
}