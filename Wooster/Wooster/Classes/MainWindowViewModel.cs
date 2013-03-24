using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
                this.AvailableActions.Add(new WoosterAction(actionName1, () => Console.WriteLine(actionName1)));
                this.AvailableActions.Add(new WoosterAction(actionName2, () => Console.WriteLine(actionName2)));
            }

            this.IsPopupOpen = this.AvailableActions.Count > 0;
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
    }
}
