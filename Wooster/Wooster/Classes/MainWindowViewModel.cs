using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        private Cache _cache;
        private Calculator _calculator = new Calculator();

        public MainWindowViewModel()
        {
            this.Config = Config.Load();
            this._cache = Cache.Load();
            this.IsPopupOpen = false;
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
                this._cache.AllActions
                    .Where(o => o.Name.ToLower().Contains(this.Query.ToLower()))
                    .Take(this.Config.MaxActionsShown)
                    .ToList()
                    .ForEach(o => this.AvailableActions.Add(o));

                // calculate?..
                if (this._calculator.LooksLikeMath(this.Query))
                {
                    var result = this._calculator.Compute(this.Query);
                    if (result != null)
                    {
                        this.AvailableActions.Insert(0, new WoosterAction(string.Format("Result: {0:F2}", result), null) { Icon = this._calculator.Icon });
                    }
                }

                if (this.Config.AutoSelectFirstAvailableAction && this.AvailableActions.Count > 0)
                {
                    this.SelectedAction = this.AvailableActions.First();
                }
            }

            this.IsPopupOpen = this.AvailableActions.Count > 0;
        }
        
        private void RecacheData()
        {
            this._cache.RecacheData();
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
