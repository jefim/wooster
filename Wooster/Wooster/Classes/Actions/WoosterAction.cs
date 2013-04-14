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
        private string _searchableName;
        private ImageSource _icon;
        private bool _includeQueryInDisplayName;

        public WoosterAction()
        {
            this.SearchableName = string.Empty;
        }

        public WoosterAction(string name, Action<string> action, bool includeQueryInDisplayName = false, bool alwaysVisible = false)
        {
            this.SearchableName = name;
            this.Action = action;
            this._includeQueryInDisplayName = includeQueryInDisplayName;
            this.AlwaysVisible = alwaysVisible;
        }

        [XmlIgnore]
        public Action<string> Action { get; set; }

        [XmlAttribute]
        public string SearchableName
        {
            get { return this._searchableName; }
            set
            {
                this._searchableName = value;
                OnPropertyChanged("SearchableName");
            }
        }

        [XmlAttribute]
        public bool AlwaysVisible
        {
            get;
            set;
        }

        /// <summary>
        /// The name that is displayed in Wooster (and SearchableName is the one that Wooster uses to search actions).
        /// </summary>
        public virtual string GetDisplayName(string query)
        {
            if (this._includeQueryInDisplayName)
            {
                return string.Format("{0} '{1}'", this.SearchableName, query);
            }
            else { return this.SearchableName; }
        }

        public void Execute(string query)
        {
            if (this.Action == null) return;
            this.Action(query);
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
