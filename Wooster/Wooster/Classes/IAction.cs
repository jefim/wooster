using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooster.Classes
{
    public interface IAction
    {
        int OrderHint { get; set; }

        void Execute(string query);
    }
}
