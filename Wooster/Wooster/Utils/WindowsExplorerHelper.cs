using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace Wooster.Utils
{
    /// <summary>
    /// This requires shell32 and Microsoft Internet Controls dll references!
    /// </summary>
    public class WindowsExplorerHelper
    {
        private IntPtr _lastExplorerWindowPtr = IntPtr.Zero;
        private Timer _timer;

        public WindowsExplorerHelper()
        {
            _timer = new Timer();
            _timer.Interval = 500;
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IntPtr handle = GetForegroundWindow();
            // ignore wooster

            if (handle == MainWindow.GetMainWindowHandle())
            {
                return;
            }

            var shellWindows = new SHDocVw.ShellWindows();
            var shell = new Shell32.Shell();
            foreach (var window in shell.Windows())
            {
                if (window.HWND == (int)handle)
                {
                    _lastExplorerWindowPtr = handle;
                    return;
                }
            }

            _lastExplorerWindowPtr = IntPtr.Zero;
        }

        public void GetExplorerSelectedFiles()
        {
            if (_lastExplorerWindowPtr == IntPtr.Zero) { return; }
            try
            {
                var shellWindows = new SHDocVw.ShellWindows();
                List<string> selected = new List<string>();
                var shell = new Shell32.Shell();
                foreach (var window in shell.Windows())
                {
                    if (window.HWND == (int)_lastExplorerWindowPtr)
                    {

                        Shell32.FolderItems items = ((Shell32.IShellFolderViewDual2)window.Document).SelectedItems();
                        foreach (Shell32.FolderItem item in items)
                        {
                            selected.Add(item.Path);
                        }
                    }
                }
                System.Windows.MessageBox.Show(string.Join("\r\n", selected));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

    }
}
