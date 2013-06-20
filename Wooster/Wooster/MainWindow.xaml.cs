using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using wf = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wooster.Classes;
using System.Reflection;
using Wooster.Utils;
using MovablePython;

namespace Wooster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow s_mainWindow;
        private MainWindowViewModel _mainWindowViewModel;
        private wf.NotifyIcon _notifyIcon;
        private Hotkey _hotkey;
        private System.Windows.Interop.WindowInteropHelper _windowInteropHelper;

        public MainWindow()
        {
            InitializeComponent();
            this._mainWindowViewModel = new MainWindowViewModel();
            this._mainWindowViewModel.HideRequest += MainWindowViewModel_HideRequest;
            this.DataContext = this._mainWindowViewModel;
            if (s_mainWindow != null) throw new InvalidOperationException("There can be only one main window!");
            s_mainWindow = this;

            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
            this.Deactivated += MainWindow_Deactivated;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            this.LoadNotificationIcon();
            this.TextBox.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(this.MainWindow_KeyDown), true);

            this.UpdatePositionOnScreen();
            this.Loaded += (s,e) => this.SetupHotkey();
        }

        public static IntPtr GetMainWindowHandle()
        {
            if (s_mainWindow == null) return IntPtr.Zero;
            return (IntPtr)s_mainWindow.Dispatcher.Invoke(new Func<IntPtr>(() => s_mainWindow._windowInteropHelper.Handle));
        }

        private void SetupHotkey()
        {
            if (this._hotkey != null) return;

            this._hotkey = new Hotkey();
            this._hotkey.Alt = this._mainWindowViewModel.Config.HotkeyConfig.Alt;
            this._hotkey.Control = this._mainWindowViewModel.Config.HotkeyConfig.Control;
            this._hotkey.Windows = this._mainWindowViewModel.Config.HotkeyConfig.Win;
            this._hotkey.Shift = this._mainWindowViewModel.Config.HotkeyConfig.Shift;
            this._hotkey.KeyCode = this._mainWindowViewModel.Config.HotkeyConfig.Key;
            this._hotkey.Pressed += Hotkey_Pressed;
            this._windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            if (!this._hotkey.GetCanRegister(this._windowInteropHelper.Handle))
            {
                Console.WriteLine("Whoops, looks like attempts to register will fail or throw an exception, show an error/visual user feedback");
            }
            else
            {
                this._hotkey.Register(this._windowInteropHelper.Handle);
            }

            this.Hide();
        }

        void Hotkey_Pressed(object sender, System.ComponentModel.HandledEventArgs e)
        {
            if (this.IsVisible) { this.Hide(); }
            else { this.Show(); }
        }

        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void UpdatePositionOnScreen()
        {
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = SystemParameters.PrimaryScreenHeight / 4;
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;
            if (e.Key == Key.Down)
            {
                var newIdx = this.ListBoxActions.SelectedIndex + 1;
                if (newIdx >= this.ListBoxActions.Items.Count) newIdx = 0;
                this.ListBoxActions.SelectedIndex = newIdx;
                return;
            }

            if (e.Key == Key.Up)
            {
                var newIdx = this.ListBoxActions.SelectedIndex-1;
                if (newIdx < 0) newIdx = this.ListBoxActions.Items.Count - 1;
                this.ListBoxActions.SelectedIndex = newIdx;
                return;
            }

            if (e.Key == Key.Enter)
            {
                this._mainWindowViewModel.ExecuteActionCommand.Execute(null);
                return;
            }
        }

        private void LoadNotificationIcon()
        {
            this._notifyIcon = new wf.NotifyIcon();
            using (var stream = AssemblyHelper.GetEmbeddedResource(typeof(MainWindow).Assembly, "mug.ico"))
            {
                this._notifyIcon.Icon = new Icon(stream);
            }

            var settingsStripItem = new wf.ToolStripButton("Settings", null, NotifyIcon_ExitMenuItemClick);
            var exitStripItem = new wf.ToolStripButton("Exit Wooster", null, NotifyIcon_ExitMenuItemClick);
            var contextMenuStrip = new wf.ContextMenuStrip();

            contextMenuStrip.ShowImageMargin = false;
            contextMenuStrip.Items.Add(settingsStripItem);
            contextMenuStrip.Items.Add(exitStripItem);
            this._notifyIcon.ContextMenuStrip = contextMenuStrip;

            this._notifyIcon.Visible = true;
            this._notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }

        void NotifyIcon_ExitMenuItemClick(object sender, EventArgs e)
        {
            this._notifyIcon.Visible = false;
            Application.Current.Shutdown();
        }

        void NotifyIcon_SettingsMenuItemClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == wf.MouseButtons.Left)
            {
                this.Show();
            }
        }

        void MainWindowViewModel_HideRequest(object sender, EventArgs e)
        {
            this.Hide();
        }

        void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this._mainWindowViewModel.OnDeactivated();
            this.Hide();
        }

        void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this._mainWindowViewModel.ExecuteActionCommand.Execute((sender as ListBoxItem).DataContext);
        }

        void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                var act = this.Activate();
                this.UpdatePositionOnScreen();
                this.Topmost = true;  // important
                this.Topmost = false; // important
                this.Focus();         // important

                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this._mainWindowViewModel.OnActivated();
                    Keyboard.Focus(this.TextBox);
                }, DispatcherPriority.Normal);
            }
        }

        internal void Exit()
        {
            if (this._hotkey.Registered) { this._hotkey.Unregister(); }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
