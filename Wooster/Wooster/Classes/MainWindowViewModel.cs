using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Wooster.ActionProviders;
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
        private IAction _selectedAction;
        private DateTime _lastQueryStringChange = DateTime.MinValue;
        private List<IActionProvider> _providers;

        public static string CurrentQuery { get; private set; }

        public MainWindowViewModel()
        {
            this.Config = Config.Load();
            this.IsPopupOpen = false;
            this.AvailableActions = new ObservableCollection<IAction>();
            this._providers = new List<IActionProvider>();
            this._providers.Add(new LocalProgramsActionProvider());
            this._providers.Add(new CalculatorActionProvider());
            foreach (var provider in this._providers)
            {
                provider.Initialize(this.Config);
            }
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
                CurrentQuery = value;
                OnPropertyChanged("Query");

                this._lastQueryStringChange = DateTime.Now;
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(200);
                    if ((DateTime.Now - this._lastQueryStringChange).TotalMilliseconds >= 200) this.RefreshActions();
                });
            }
        }

        private void RefreshActions()
        {
            List<IAction> outputActions = new List<IAction>();
            if (!string.IsNullOrWhiteSpace(this.Query))
            {                
                outputActions = this._providers
                    .SelectMany(o => o.GetActions(this.Query))
                    .Take(this.Config.MaxActionsShown)
                    .OrderBy(o => o.OrderHint)
                    .ToList();
            }

            // Update UI
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.AvailableActions.Clear();
                foreach (var action in outputActions) this.AvailableActions.Add(action);

                if (this.Config.AutoSelectFirstAvailableAction && this.AvailableActions.Count > 0)
                {
                    this.SelectedAction = this.AvailableActions.First();
                }

                this.IsPopupOpen = this.AvailableActions.Count > 0;
            }));
        }
        
        private void RecacheData()
        {
            foreach (var provider in this._providers)
            {
                provider.RecacheData();
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
        
        private void Hide()
        {
            var evt = this.HideRequest;
            if (evt != null) { evt(this, EventArgs.Empty); }
        }

        public ObservableCollection<IAction> AvailableActions
        {
            get;
            private set;
        }

        public IAction SelectedAction
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
            this.SelectedAction.Execute(this.Query);
            this.DeactivateCommand.Execute(null);
        }

        #endregion
    }
}
