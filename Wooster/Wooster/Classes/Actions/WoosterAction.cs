using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using Wooster.Utils;

namespace Wooster.Classes.Actions
{
    public class WoosterAction : Observable
    {
        private string _name;
        private ImageSource _icon;

        public WoosterAction()
        {
            this.Name = string.Empty;
        }

        public WoosterAction(string name, Action action)
        {
            this.Name = name;
            this.Action = action;
        }

        [XmlIgnore]
        public Action Action { get; set; }

        [XmlAttribute]
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

        [XmlIgnore]
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
