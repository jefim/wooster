using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wooster.Utils
{
    public class Calculator
    {
        private DataTable _dataTable = new DataTable();
        private char[] mathOperators = new[] { '+', '-', '*', '/', '(', ')' };
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
                return this._dataTable.Compute(expression, null).ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
