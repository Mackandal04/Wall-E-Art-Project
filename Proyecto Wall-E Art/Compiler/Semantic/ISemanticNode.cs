using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Wall_E_Art
{
    public interface ISemanticNode
    {
        void Validate(SemanticContext context);
    }
}