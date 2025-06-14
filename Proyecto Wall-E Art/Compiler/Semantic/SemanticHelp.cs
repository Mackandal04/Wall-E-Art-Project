using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public static class SemanticHelp
    {
        public static void CheckParams(SemanticContext semanticContext, string instruccionName, string expType, params (string name, ExpressionNode expressionNode)[] parameters)
        {
            foreach (var item in parameters)
            {
                var actualType = item.expressionNode.CheckType(semanticContext);
                if(actualType != expType)
                {
                    semanticContext.GetErrors($"'{instruccionName}': el parametro '{item.name}', debe ser '{expType}' y no '{actualType}'.", item.expressionNode.Line);
                }
            }
        }
    }
}