using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public class SemanticContext
    {
        public Dictionary<string, string> VariablesTable { get; } = new Dictionary<string, string>();

        public Dictionary<string, int> LabelsTable { get; } = new Dictionary<string, int>();

        public string[]ColorsTable { get; } = new string[]
        {
            "Red", "Green", "Blue", "Yellow", "Black", "White", "Orange", "Purple", "Transparent"
        };

        public List<SemanticError> Errors { get; } = new List<SemanticError>();

        public void GetErrors(string message, int line) => Errors.Add(new SemanticError(message, line));
    }
}