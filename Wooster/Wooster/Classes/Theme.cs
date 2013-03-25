using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooster.Classes
{
    public class Theme
    {
        public double FontSize { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }

        public double ActionListFontSize { get; set; }
        public string ActionListBackgroundColor { get; set; }
        public string ActionListForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of alternating elements' background in the action list.
        /// (= every second action will have background painted with this color)
        /// </summary>
        public string ActionListAlternateBackgroundColor { get; set; }

        public Theme()
        {
            this.FontSize = 36;
            this.BorderColor = "#444444";
            this.BackgroundColor = "#FFFFFF";

            this.ActionListFontSize = 22;
            this.ActionListBackgroundColor = "#EEEEEE";
            this.ActionListForegroundColor = "#333333";
            this.ActionListAlternateBackgroundColor = "#FAFAFA";
        }
    }
}
