    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Proyecto_Wall_E_Art;

    CompilerDriver compilerDriver  = new CompilerDriver("@x+3");

    foreach (var item in compilerDriver.tokens)
    {
        System.Console.WriteLine(item.Kind);
    }