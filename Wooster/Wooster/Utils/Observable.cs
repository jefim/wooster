using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Wooster.Utils
{
    public class Observable : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
            var evt = this.PropertyChanged;
            if (evt != null) { evt(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
