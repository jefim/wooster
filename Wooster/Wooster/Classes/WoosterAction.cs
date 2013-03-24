using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wooster.Utils;

namespace Wooster.Classes
{
    public class WoosterAction : Observable
    {
        private string _name;

        public WoosterAction(string name, Action action)
        {
            this.Name = name;
            this.Action = action;
        }

        public Action Action { get; set; }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                OnPropertyChanged("Name");
            }
        }
    }
}
