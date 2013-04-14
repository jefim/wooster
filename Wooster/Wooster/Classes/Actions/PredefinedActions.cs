using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Wooster.Classes.Actions
{
    public static class PredefinedActions
    {
        public static IEnumerable<WoosterAction> GetPredefinedActions()
        {
            yield return new WoosterAction("Search Google for ", s => { Process.Start("http://www.google.fi/search?q=" + s); }, true, true);
        }
    }
}
