using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public static class ErrorsCollecter
    {
        static List<Error> errors = new List<Error>();

        public static void Add(string type, string message, int line)
        {
            errors.Add(new Error(type, message, line));
        }

        public static void ErrorsClear()
        {
            errors.Clear();
        }

        public static List<Error> GetErrors()
        {
            return errors;
        }

        public static bool HasErrors()
        {
            if (errors.Count > 0)
                return true;

            return false;
        }
        
    }
}