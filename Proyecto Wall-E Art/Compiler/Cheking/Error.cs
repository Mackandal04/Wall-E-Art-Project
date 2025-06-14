using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public class Error
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public int Line { get; set; }

        public Error(string type, string message, int line)
        {
            Type = type;
            Message = message;
            Line = line;
        }

        public override string ToString()
        {
            return $" [{Type}] Linea {Line} : {Message}";
        }
    }
}