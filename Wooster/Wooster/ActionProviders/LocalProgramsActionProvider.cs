using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wooster.Classes;

namespace Wooster.ActionProviders
{
    public class LocalProgramsActionProvider : IActionProvider
    {
        private Cache _cache;
        private Config _config;
           
        public void Initialize(Config config)
        {
            this._config = config;
            this._cache = Cache.Load();
        }

        public IEnumerable<IAction> GetActions(string queryString)
        {
            var outputActions = new List<IAction>();

            // search by first letters
            // @"\b{CHUNK1}.*\b"
            // @"\b{CHUNK1}.*?\b{CHUNK2}.*\b"
            var chunks = queryString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Regex regex = new Regex(string.Format(@"\b{0}.*\b", string.Join(@".*?\b", chunks)), RegexOptions.IgnoreCase);

            this._cache.AllActions
                .Where(o =>
                {
                    // search by contains
                    if (o.AlwaysVisible || o.SearchableName.ToLower().Contains(queryString.ToLower())) return true;

                    if (this._config.SearchByFirstLettersEnabled)
                    {
                        // search by first letters
                        return regex.IsMatch(o.SearchableName);
                    }
                    else return false;
                })
                .Take(this._config.MaxActionsShown)
                .OrderBy(o => o.OrderHint)
                .ThenBy(o => o.SearchableName)
                .ToList()
                .ForEach(o => outputActions.Add(o));

            return outputActions;
        }


        public void RecacheData()
        {
            this._cache.RecacheData();
        }
    }
}
