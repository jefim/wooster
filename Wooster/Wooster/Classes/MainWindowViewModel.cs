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
        private RelayCommand _executeActionCommand;
        internal event EventHandler HideRequest;
        private WoosterAction _selectedAction;
        private List<WoosterAction> _cachedActions = new List<WoosterAction>();
        private List<WoosterAction> _defaultActions;

        public MainWindowViewModel()
        {
            this.Config = Config.Load();
            this.IsPopupOpen = false;
            this._defaultActions = new List<WoosterAction> { new WoosterAction("Recache Wooster data", () => this.RecacheData()) };
            this._cachedActions.AddRange(this._defaultActions);
            this.AvailableActions = new ObservableCollection<WoosterAction>();
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
                this._cachedActions
                    .Where(o => o.Name.ToLower().Contains(this.Query.ToLower()))
                    .OrderBy(o => o.Name)
                    .ToList()
                    .ForEach(o => this.AvailableActions.Add(o));

                if (this.Config.AutoSelectFirstAvailableAction && this.AvailableActions.Count > 0)
                {
                    this.SelectedAction = this.AvailableActions.First();
                }
            }

            this.IsPopupOpen = this.AvailableActions.Count > 0;
        }
        
        private void RecacheData()
        {
            this._cachedActions.Clear();
            this._cachedActions.AddRange(this.RecacheProgramShortcutData());
            this._cachedActions.AddRange(this._defaultActions);
        }

        private IEnumerable<ProgramShortcutAction> RecacheProgramShortcutData()
        {
            var result = new List<ProgramShortcutAction>();

            var pathToUserStartMenu = Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Windows\Start Menu\Programs");
            foreach (var file in Directory.EnumerateFiles(pathToUserStartMenu))
            {
                if (file.EndsWith(".lnk", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Add(new ProgramShortcutAction(file));
                }
            }

            return result;
        }

        internal void OnActivated()
        {
        }

        internal void OnDeactivated()
        {
            this.IsPopupOpen = false;
            this.Query = string.Empty;
        }
        
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

        public WoosterAction SelectedAction
        {
            get { return this._selectedAction; }
            set
            {
                this._selectedAction = value;
                OnPropertyChanged("SelectedAction");
            }
        }

        #region DeactivateCommand

        /// <summary>
        /// Activated when user e.g. presses escape
        /// </summary>
        public RelayCommand DeactivateCommand
        {
            get
            {
                return this._deactivateCommand ??
                    (this._deactivateCommand = new RelayCommand(param => { this.OnDeactivated(); this.Hide(); }));
            }
        }

        #endregion

        #region ExecuteActionCommand

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
            this.SelectedAction.Execute();
            this.DeactivateCommand.Execute(null);
        }

        #endregion
    }
}
