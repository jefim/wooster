using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wooster.Classes;

namespace Wooster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            this._mainWindowViewModel = new MainWindowViewModel();
            this._mainWindowViewModel.HideRequest += MainWindowViewModel_HideRequest;
            this.DataContext = this._mainWindowViewModel;

            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
            this.Deactivated += MainWindow_Deactivated;
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
                }, DispatcherPriority.Render);
            }
        }

        private void Popup_Opened_1(object sender, EventArgs e)
        {

        }
    }
}
