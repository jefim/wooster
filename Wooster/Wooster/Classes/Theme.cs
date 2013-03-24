using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooster.Classes
{
    public class Theme
    {
        public double FontSize { get; set; }
        public string BorderBrush { get; set; }

        public double ActionListFontSize { get; set; }
        public string ActionListBorderBrush { get; set; }

        public Theme()
        {
            this.FontSize = 36;
            this.BorderBrush = "#777777";

            this.ActionListFontSize = 26;
            this.ActionListBorderBrush = "#999999";
        }
    }
}
