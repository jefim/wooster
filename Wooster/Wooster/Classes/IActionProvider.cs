using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooster.Classes
{
    public interface IActionProvider
    {
        void Initialize(Config config);
        IEnumerable<IAction> GetActions(string queryString);
        void RecacheData();
    }
}
