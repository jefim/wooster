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

namespace Wooster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;
        private wf.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            this._mainWindowViewModel = new MainWindowViewModel();
            this._mainWindowViewModel.HideRequest += MainWindowViewModel_HideRequest;
            this.DataContext = this._mainWindowViewModel;

            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
            this.Deactivated += MainWindow_Deactivated;

            this.LoadNotificationIcon();
        }

        private void LoadNotificationIcon()
        {
            var asm = typeof(MainWindow).Assembly;
            var resources = asm.GetManifestResourceNames();
            var icon = resources.FirstOrDefault(o => o.EndsWith("mug.ico"));
            using (var stream = asm.GetManifestResourceStream(icon))
            {
                this._notifyIcon = new wf.NotifyIcon();
                this._notifyIcon.Icon = new Icon(stream);

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
    }
}
