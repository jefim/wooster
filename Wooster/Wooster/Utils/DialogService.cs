using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Wooster.Utils
{
    public static class DialogService
    {
        public static void ShowDialog(ViewModelBase viewModel, double width, double height)
        {
            Window window = new Window();
            window.Content = new ContentPresenter { Content = viewModel };
            window.Title = viewModel.DisplayName;
            window.Width = width;
            window.Height = height;
            window.ShowDialog();
        }
    }
}
