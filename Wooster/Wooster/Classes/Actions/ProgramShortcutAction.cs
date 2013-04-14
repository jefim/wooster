using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Wooster.Utils;

namespace Wooster.Classes.Actions
{
    public class ProgramShortcutAction : WoosterAction
    {
        private string _shortcutPath;

        public ProgramShortcutAction() : base()
        {
        }

        public ProgramShortcutAction(string shortcutPath)
            : base("", null)
        {
            this.ShortcutPath = shortcutPath;
        }
        
        [XmlAttribute]
        public string ShortcutPath
        {
            get { return this._shortcutPath; }
            set
            {
                this._shortcutPath = value;
                this.Load();
            }
        }

        [XmlIgnore]
        public string RealPath
        {
            get;
            private set;
        }

        private void Load()
        {
            if (this.ShortcutPath == null)
            {
                this.Icon = null;
                this.Action = null;
                this.SearchableName = string.Empty;
                return;
            }

            var shellFile = ShellFile.FromFilePath(this.ShortcutPath);
            this.RealPath = shellFile.Properties.System.Link.TargetParsingPath.Value;
            var icon = WindowsShortcut.GetIconForFile(this.RealPath, ShellIconSize.LargeIcon);
            if (icon == null) icon = WindowsShortcut.GetIconForFile(this.ShortcutPath, ShellIconSize.LargeIcon); // try getting the icon from shortcut if failed getting from target
            if (icon != null) this.Icon = icon.ToImageSource();
            this.SearchableName = Path.GetFileNameWithoutExtension(this.ShortcutPath);
            this.Action = s => Process.Start(this.ShortcutPath);
        }
    }
}
