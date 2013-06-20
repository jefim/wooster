using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Wooster.Classes.Actions;
using Wooster.Utils;
using Wooster.ViewModels;

namespace Wooster.Classes
{
    public class Cache
    {
        private List<WoosterAction> _defaultActions;
        private WindowsExplorerHelper _windowsExplorerHelper = new WindowsExplorerHelper();

        public Cache()
        {
            this._defaultActions = new List<WoosterAction>(PredefinedActions.GetPredefinedActions());
            this._defaultActions.Add(new WoosterAction("Recache Wooster data", s => this.RecacheData()));
            this._defaultActions.Add(new WoosterAction("Test Windows Explorer", s => this._windowsExplorerHelper.GetExplorerSelectedFiles()));
            //this._defaultActions.Add(new WoosterAction("Open Wooster preferences", s=> DialogService.ShowDialog(new PreferencesViewModel(), 700, 600)));
            this._defaultActions.Add(new WoosterAction("Open Wooster config folder", s => Process.Start(Path.GetDirectoryName(Config.GetRealConfigPath()))));

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

        // DO NOT CHANGE BACK TO "private set;" - this makes it crash in .NET 4.0!
        public List<ProgramShortcutAction> ProgramShortcutActions { get; set; }

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
