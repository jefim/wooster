using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        public static string CurrentQuery { get; private set; }

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
                CurrentQuery = value;
                OnPropertyChanged("Query");

                this._lastQueryStringChange = DateTime.Now;
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(200);
                    if ((DateTime.Now - this._lastQueryStringChange).TotalMilliseconds >= 200) this.RefreshActions();
                });
                //this.RefreshActions();
            }
        }

        private DateTime _lastQueryStringChange = DateTime.MinValue;
        private Task _refreshActionsTask = null;

        private void RefreshActions()
        {
            var outputActions = new List<WoosterAction>();
            //this.AvailableActions.Clear();

            if (!string.IsNullOrWhiteSpace(this.Query))
            {
                // search by first letters
                // @"\b{CHUNK1}.*\b"
                // @"\b{CHUNK1}.*?\b{CHUNK2}.*\b"
                var chunks = this.Query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Regex regex = new Regex(string.Format(@"\b{0}.*\b", string.Join(@".*?\b", chunks)), RegexOptions.IgnoreCase);
                this._cache.AllActions
                    .Where(o =>
                    {
                        // search by contains
                        if (o.AlwaysVisible || o.SearchableName.ToLower().Contains(this.Query.ToLower())) return true;

                        if (this.Config.SearchByFirstLettersEnabled)
                        {
                            // search by first letters
                            return regex.IsMatch(o.SearchableName);
                        }
                        else return false;
                    })
                    .Take(this.Config.MaxActionsShown)
                    .OrderBy(o => o.OrderHint)
                    .ThenBy(o => o.SearchableName)
                    .ToList()
                    .ForEach(o => outputActions.Add(o));                

                // calculate?..
                if (this._calculator.LooksLikeMath(this.Query))
                {
                    var result = this._calculator.Compute(this.Query);
                    if (result != null)
                    {
                        outputActions.Insert(0, new WoosterAction(
                            string.Format("Copy result to clipboard: {0}", result), s => Clipboard.SetText(result)) { Icon = this._calculator.Icon });
                    }
                }
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
            this.SelectedAction.Execute(this.Query);
            this.DeactivateCommand.Execute(null);
        }

        #endregion
    }
}
