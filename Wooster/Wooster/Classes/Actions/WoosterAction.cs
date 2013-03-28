using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Wooster.Utils;

namespace Wooster.Classes.Actions
{
    public class WoosterAction : Observable
    {
        private string _name;
        private ImageSource _icon;

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

        public void Execute()
        {
            if (this.Action == null) return;
            this.Action();
        }

        public ImageSource Icon
        {
            get { return this._icon; }
            set
            {
                this._icon = value;
                OnPropertyChanged("Icon");
            }
        }
    }
}
