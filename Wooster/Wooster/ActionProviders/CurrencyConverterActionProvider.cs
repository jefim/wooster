using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Wooster.Classes;
using Wooster.Classes.Actions;

namespace Wooster.ActionProviders
{
    internal class CurrencyConverterActionProvider : IActionProvider
    {
        private class CurrencyConverter
        {
            private static List<string> Currencies = new List<string> {
"AED", "AFN", "ALL", "AMD", "ARS", "AUD", "AZN", "BAM", "BDT", "BGN", "BHD", "BND", "BOB", "BRL", "BYR", "BZD", "CAD", "CHF", "CLP", "CNY", "COP", "CRC", "CSD", "CZK", "DKK", "DOP", "DZD", "EEK", "EGP", "ETB", "EUR", "GBP", "GEL", "GTQ", "HKD", "HNL", "HRK", "HUF", "IDR", "ILS", "INR", "IQD", "IRR", "ISK", "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KRW", "KWD", "KZT", "LAK", "LBP", "LKR", "LTL", "LVL", "LYD", "MAD", "MKD", "MNT", "MOP", "MVR", "MXN", "MYR", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUR", "RUB", "RWF", "SAR", "SEK", "SGD", "SYP", "THB", "TJS", "TMT", "TND", "TRY", "TTD", "TWD", "UAH", "USD", "UYU", "UZS", "VEF", "VND", "XOF", "YER", "ZAR", "ZWL"
};
            //http://motyar.blogspot.fi/2011/12/googles-currency-converter-and-json-api.html

            public string ConvertString(string searchQuery)
            {
                // Let's see if query is relevant to this provider
                // Example string that we handle: "20 eur to usd"
                var pieces = searchQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length == 4 && Currencies.Contains(pieces[1].ToUpper()) && Currencies.Contains(pieces[3].ToUpper()))
                {
                    double amount;
                    if (double.TryParse(pieces[0].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                    {
                        return this.Convert(amount, pieces[1], pieces[3]); //"try to convert " + pieces[0];
                    }
                }

                return null;
            }

            public string Convert(double amount, string from, string to)
            {
                var queryString = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://www.google.com/ig/calculator?hl=en&lang=ru&q={0}{1}=?{2}",
                    amount,
                    from,
                    to);
                using (var webClient = new WebClient())
                {
                    // example result from google:
                    // {lhs: "1 British pound",rhs: "1.5718 U.S. dollars",error: "",icc: true}
                    var result = webClient.DownloadString(queryString);
                    result = result.Trim('{', '}');
                    var pcs = result.Split(',').Select(o => o.Trim('"', ' ')).ToArray();
                    return string.Format("{0} = {1}", pcs[0].Substring(6), pcs[1].Substring(6));
                }
            }
        }

        private static CurrencyConverter CurrencyConverterInst = new CurrencyConverter();

        public void Initialize(Config config)
        {
            // do nothing
        }

        public IEnumerable<IAction> GetActions(string queryString)
        {
            var result = CurrencyConverterInst.ConvertString(queryString);
            if (result == null) yield break;
            else yield return new WoosterAction(result, null);
        }

        public void RecacheData()
        {
            // do nothing
        }
    }

}
