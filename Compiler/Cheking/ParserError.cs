using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public static class ParserError
    {
        static List<string> errors = new List<string>();

        public static void Report(string message, int line, int column)
        {
            errors.Add($"Línea {line}, Columna {column}: {message}");
        }

        public static IEnumerable<string> Errors => errors;

        public static void Clear() => errors.Clear();
    }
}