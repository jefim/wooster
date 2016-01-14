using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Wooster.Classes;
using Wooster.Classes.Actions;
using Wooster.Lib;
using Wooster.Utils;

namespace Wooster.ActionProviders
{
    public class MiscActionProvider : IActionProvider
    {
        private Config _config;
        private WoosterAction _createFolderWithSelectedFilesCommand;
        private WindowsExplorerHelper _windowsExplorerHelper;
        private IPromptService _promptService;

        public MiscActionProvider(WindowsExplorerHelper windowsExplorerHelper, IPromptService promptService)
        {
            this._windowsExplorerHelper = windowsExplorerHelper;
            this._promptService = promptService;
        }

        public IEnumerable<IAction> GetActions(string queryString)
        {
            var selectedFiles = this._windowsExplorerHelper.GetExplorerSelectedFiles();
            if (selectedFiles.Any() && this.CreateFolderWithSelectedFilesCommand.MatchesQueryString(queryString, this._config != null && this._config.SearchByFirstLettersEnabled))
            {
                yield return this.CreateFolderWithSelectedFilesCommand;
            }
        }

        public void Initialize(Config config)
        {
            this._config = config;
        }

        public void RecacheData()
        {
        }

        public WoosterAction CreateFolderWithSelectedFilesCommand
        {
            get
            {
                if(this._createFolderWithSelectedFilesCommand == null)
                {
                    this._createFolderWithSelectedFilesCommand = new WoosterAction("New folder with selected files", s =>
                    {
                        var selectedFiles = this._windowsExplorerHelper.GetExplorerSelectedFiles();
                        var folder = selectedFiles.Select(o => Path.GetDirectoryName(o)).Distinct().First();
                        var name = this._promptService.AskForText("Enter name for the new directory", "Prompt");
                        if (!(string.IsNullOrWhiteSpace(name)))
                        {
                            var newFolder = Path.Combine(folder, name);
                            this.CreateFolderWithFiles(selectedFiles, newFolder);
                        }
                    });
                }

                return this._createFolderWithSelectedFilesCommand;
            }
        }

        private void CreateFolderWithFiles(IEnumerable<string> files, string newFolderPath)
        {
            var distinctDirs = files.Select(o => Path.GetDirectoryName(o)).Distinct().ToList();
            if(Directory.Exists(newFolderPath))
            {
                var confirm = this._promptService.Confirm(
                    "A directory with this name already exists - move files into this directory?",
                    "Directory exists");
                if (!confirm) return;
            }
            Directory.CreateDirectory(newFolderPath);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                File.Move(file, Path.Combine(newFolderPath, fileName));
            }
        }
    }
}
