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
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            this.LoadNotificationIcon();
            TextBox.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(this.MainWindow_KeyDown), true);

            this.UpdatePositionOnScreen();
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
    }
}
