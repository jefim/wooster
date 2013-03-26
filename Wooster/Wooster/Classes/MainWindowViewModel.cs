using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wooster.Classes.Actions;
using Wooster.Utils;

namespace Wooster.Classes
{
    public class MainWindowViewModel : Observable
    {
        private bool _isPopupOpen;
        private string _query;
        private RelayCommand _deactivateCommand;

        public MainWindowViewModel()
        {
            this.Config = Config.Load();
            this.AvailableActions = new ObservableCollection<WoosterAction>();
            this.IsPopupOpen = false;
        }

        public Config Config { get; private set; }

        public bool IsPopupOpen
        {
            get { return this._isPopupOpen; }
            set
            {
                this._isPopupOpen = value;
                OnPropertyChanged("IsPopupOpen");
            }
        }
        
        public string Query
        {
            get { return this._query; }
            set
            {
                if (this._query == value) return;
                this._query = value;
                OnPropertyChanged("Query");
                this.RefreshActions();
            }
        }

        private void RefreshActions()
        {
            this.AvailableActions.Clear();

            if (!string.IsNullOrWhiteSpace(this.Query))
            {
                var actionName1 = "Act now: " + this.Query;
                var actionName2 = "Fap now: " + this.Query;
                var actionName3 = "Zuzizazaaaa: " + this.Query;
                var actionName4 = "Minorabama: " + this.Query;
                var actions = new List<WoosterAction>();
                this.AvailableActions.Add(new WoosterAction(actionName1, () => Console.WriteLine(actionName1)));
                this.AvailableActions.Add(new WoosterAction(actionName2, () => Console.WriteLine(actionName2)));
                this.AvailableActions.Add(new WoosterAction(actionName3, () => Console.WriteLine(actionName3)));
                this.AvailableActions.Add(new WoosterAction(actionName4, () => Console.WriteLine(actionName4)));
                this.AvailableActions.Add(new WoosterAction("Recache data", () => this.RecacheData()));


                this.IsPopupOpen = this.AvailableActions.Count > 0;
            }
        }

        private void RecacheData()
        {
            var pathToUserStartMenu = Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Windows\Start Menu\Programs");
            foreach(var file in Directory.EnumerateFiles(pathToUserStartMenu)) {
                if (file.EndsWith(".lnk", StringComparison.CurrentCultureIgnoreCase))
                {
                    var icon = WindowsShortcut.GetIconForFile(file, ShellIconSize.SmallIcon);
                }
            }
        }

        internal void OnActivated()
        {
        }

        internal void OnDeactivated()
        {
            this.IsPopupOpen = false;
            this.Query = string.Empty;
        }

        internal event EventHandler HideRequest;
        private RelayCommand _executeActionCommand;

        private void Hide()
        {
            var evt = this.HideRequest;
            if (evt != null) { evt(this, EventArgs.Empty); }
        }

        public ObservableCollection<WoosterAction> AvailableActions
        {
            get;
            private set;
        }

        public RelayCommand DeactivateCommand
        {
            get
            {
                return this._deactivateCommand ??
                    (this._deactivateCommand = new RelayCommand(param => { this.OnDeactivated(); this.Hide(); }));
            }
        }

        public RelayCommand ExecuteActionCommand
        {
            get
            {
                return this._executeActionCommand ??
                    (this._executeActionCommand = new RelayCommand(this.ExecuteActionCommand_Executed));
            }
        }

        private void ExecuteActionCommand_Executed(object obj)
        {
            WoosterAction action = obj as WoosterAction;
            action.Execute();            
        }
    }
}
