using MovablePython;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wooster.Utils;

namespace Wooster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Hotkey _hotkey;
        MainWindow _window = new MainWindow();
        private System.Windows.Interop.WindowInteropHelper _windowInteropHelper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // yay!
            this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            this.SetupHotkey();
        }

        private void SetupHotkey()
        {

            this._hotkey = new Hotkey();
            this._hotkey.Alt = true;
            this._hotkey.KeyCode = Keys.Space;
            this._hotkey.Pressed += Hotkey_Pressed;
            this._window.Show();
            this._windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this._window);
            if (!this._hotkey.GetCanRegister(this._windowInteropHelper.Handle))
            {
                Console.WriteLine("Whoops, looks like attempts to register will fail or throw an exception, show an error/visual user feedback");
            }
            else
            {
                this._hotkey.Register(this._windowInteropHelper.Handle);
            }

            this._window.Hide();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (this._hotkey.Registered)
            {
                this._hotkey.Unregister();
            }
        }

        void Hotkey_Pressed(object sender, System.ComponentModel.HandledEventArgs e)
        {
            if (this._window.IsVisible) { this._window.Hide(); }
            else { this._window.Show(); }
        }
    }
}
