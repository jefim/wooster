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
            var outputActions = this._cache.AllActions
                .Where(o =>
                {
                    return o.MatchesQueryString(queryString, this._config != null && this._config.SearchByFirstLettersEnabled);
                })
                .OrderBy(o => o.OrderHint)
                .ThenBy(o => o.SearchableName)
                .ToList();

            return outputActions;
        }

        public void RecacheData()
        {
            this._cache.RecacheData();
        }
    }
}
