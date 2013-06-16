using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Wooster.Classes.Actions
{
    public class ExecuteWindowsExplorerCommandAction : WoosterAction
    {
        public string Command { get; set; }

        public ExecuteWindowsExplorerCommandAction() : base("ExecuteWindowsExplorerCommandAction", null) 
        {
            this.Action = (s) => Process.Start(this.Command.Replace("{query}", s));
        }

        public ExecuteWindowsExplorerCommandAction(string name, string command) : base (name, null)
        {
        }

        
    }
}
