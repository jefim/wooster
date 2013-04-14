using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Wooster.Classes;
using Wooster.Classes.Actions;

namespace Wooster.Utils
{
    public class GetDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var waction = value as WoosterAction;
            if (waction == null) return null;
            return waction.GetDisplayName(MainWindowViewModel.CurrentQuery);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Not supported!");
        }
    }
}
