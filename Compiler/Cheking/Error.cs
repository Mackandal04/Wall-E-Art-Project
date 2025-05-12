using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto_Wall_E_Art;

namespace Proyecto_Wall_E_Art
{
    public static class Error
    {
        public static bool wrong = false;
        public static string message = "";
        public static string typeMessage = "";

        public static void SetError(string type, string msg)
        {
            if (wrong) return;
            wrong = true;
            message = msg;
            typeMessage = type;
        }
    }
}