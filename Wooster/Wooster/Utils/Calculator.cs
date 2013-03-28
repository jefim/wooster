using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wooster.Utils
{
    public class Calculator
    {
        private DataTable _dataTable = new DataTable();
        private List<char> mathOperators = new List<char> { '+', '-', '*', '/', '(', ')', '.', ',' };
        private List<char> dotAndComma = new List<char> { '.', ',' };
        private ImageSource _icon;

        public Calculator()
        {
            using (var stream = AssemblyHelper.GetEmbeddedResource(typeof(Calculator).Assembly, "calculate.png"))
            {
                this._icon = ImageSourceHelper.StreamToImageSource(stream);
            }
        }

        public bool LooksLikeMath(string expression)
        {
            // if all symbols are digits, dot and commas - this is not an expression
            if (expression.All(o => char.IsDigit(o) || dotAndComma.Contains(o))) return false;

            return expression.All(o => char.IsDigit(o) || char.IsWhiteSpace(o) || mathOperators.Contains(o));
        }

        public ImageSource Icon
        {
            get
            {
                return this._icon;
            }
        }

        public string Compute(string expression)
        {
            try
            {
                expression = expression.Replace(',', '.');
                return string.Format(CultureInfo.CurrentCulture, "{0}", this._dataTable.Compute(expression, null));
            }
            catch
            {
                return null;
            }
        }
    }
}
