﻿using MovablePython;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
        MainWindow _window = new MainWindow();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // yay!
            var sw = Stopwatch.StartNew();
            this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            this._window.Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this._window.Exit();
        }
    }
}
