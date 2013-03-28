using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Wooster.Utils;

namespace Wooster.Classes.Actions
{
    public class ProgramShortcutAction : WoosterAction
    {
        public ProgramShortcutAction(string shortcutPath)
            : base("", null)
        {
            var shellFile = ShellFile.FromFilePath(shortcutPath);
            //var iconStuff = shellFile.Properties.DefaultPropertyCollection.Where(o => o.IconReference != null).ToList();
            //var dict = shellFile.Properties.DefaultPropertyCollection.ToDictionary(o => o.CanonicalName ?? Guid.NewGuid().ToString(), o => o.ValueAsObject == null ? "<null>" : o.ValueAsObject.ToString());
            var realPath = shellFile.Properties.System.Link.TargetParsingPath.Value;
            var icon = WindowsShortcut.GetIconForFile(realPath, ShellIconSize.LargeIcon);
            this.Icon = icon.ToImageSource();
            this.Name = Path.GetFileNameWithoutExtension(shortcutPath);
            this.Action = () => Process.Start(shortcutPath);
        }
    }
}
