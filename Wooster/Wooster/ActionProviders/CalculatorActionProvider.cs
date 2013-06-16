using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Wooster.Classes;
using Wooster.Classes.Actions;
using Wooster.Utils;

namespace Wooster.ActionProviders
{
    public class CalculatorActionProvider : IActionProvider
    {
        private Calculator _calculator = new Calculator();

        public void Initialize(Config config)
        {
            // do nothing
        }

        public IEnumerable<IAction> GetActions(string queryString)
        {
            // calculate?..
            if (this._calculator.LooksLikeMath(queryString))
            {
                var result = this._calculator.Compute(queryString);
                if (result != null)
                {
                    var action = new WoosterAction(string.Format("Copy result: {0}", result), s => Clipboard.SetText(result)) { Icon = this._calculator.Icon };
                    yield return action;
                }
            }
        }

        public void RecacheData()
        {
            // do nothing
        }
    }
}
