using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Wooster.Classes.Actions;

namespace Wooster.Classes
{
    public class Cache
    {
        private List<WoosterAction> _defaultActions;

        public Cache()
        {
            this._defaultActions = new List<WoosterAction>(PredefinedActions.GetPredefinedActions());
            this._defaultActions.Add(new WoosterAction("Recache Wooster data", s => this.RecacheData()));
            this.ProgramShortcutActions = new List<ProgramShortcutAction>();
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Cache ));
            using (var writer = XmlTextWriter.Create(GetCachePath(), new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, this);
            }
        }

        public List<ProgramShortcutAction> ProgramShortcutActions { get; private set; }

        public IEnumerable<WoosterAction> AllActions
        {
            get
            {
                return this._defaultActions.Union(this.ProgramShortcutActions).OrderBy(o => o.SearchableName);
            }
        }

        private static string GetCachePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = "Wooster";
            var configFileName = "Wooster.cache.xml";
            var fullAppFolder = Path.Combine(appDataPath, appFolder);
            if (!Directory.Exists(fullAppFolder)) Directory.CreateDirectory(fullAppFolder);
            return Path.Combine(appDataPath, appFolder, configFileName);
        }

        internal static Cache Load()
        {
            var cache = new Cache();
            var serializer = new XmlSerializer(typeof(Cache));
            try
            {
                using (var reader = XmlTextReader.Create(GetCachePath()))
                {
                    cache = (Cache)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
            }

            return cache;
        }
        
        private void RecacheProgramShortcutData()
        {
            var result = new List<ProgramShortcutAction>();

            // In the following lines we are doing "distinct" since a program can place its shortbut in both "All programs" and "Startup" folders
            var pathToUserStartMenu = Environment.ExpandEnvironmentVariables(@"%appdata%\Microsoft\Windows\Start Menu\Programs");
            var distinctUserShortcuts = ScanDirectoryForShortcuts(pathToUserStartMenu).GroupBy(o => o.RealPath).Select(o => o.First());
            result.AddRange(distinctUserShortcuts);

            var pathToAllUsersStartMenu = Environment.ExpandEnvironmentVariables(@"%programdata%\Microsoft\Windows\Start Menu\Programs");
            var distinctAllUsersShortcuts = ScanDirectoryForShortcuts(pathToAllUsersStartMenu).GroupBy(o => o.RealPath).Select(o => o.First());
            result.AddRange(distinctAllUsersShortcuts);

            // do this in the end so that we don't lose existing stuff if something goes wrong
            this.ProgramShortcutActions.Clear();
            this.ProgramShortcutActions.AddRange(result);
            this.Save();
        }

        /// <summary>
        /// Scans the dir for shortcuts (including subdirectories) and returns actions for those shortcuts.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        private static IEnumerable<ProgramShortcutAction> ScanDirectoryForShortcuts(string directoryPath)
        {
            List<string> extensionsToParse = new List<string> { ".lnk", ".url" };
            if (Directory.Exists(directoryPath))
            {
                foreach (var file in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
                {
                    if (!extensionsToParse.Contains(Path.GetExtension(file).ToLower())) continue;
                    yield return new ProgramShortcutAction(file);
                }
            }
        }

        internal void RecacheData()
        {
            this.RecacheProgramShortcutData();
        }
    }
}
